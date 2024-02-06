using DirWatcher.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DirWatcher.Controllers;

namespace DirWatcher.Services
{
    public class DirWatcherService
    {
        private readonly object lockObj = new object();
        private string directoryPath;
        private TimeSpan interval;
        private string magicString;
        private CancellationTokenSource cancellationTokenSource;
        private Task task;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private Task _backgroundTask;
        private bool _isTaskRunning;
        private readonly object _lockObj = new object();
        private readonly string _watchDirectory;
        private Timer _timer;
        private readonly AppDbContext _dbContext;

        public DirWatcherService()
        {
            directoryPath = ""; // Default directory path
            interval = TimeSpan.FromMinutes(1); // Default interval
            magicString = ""; // Default magic string
            _cancellationTokenSource = new CancellationTokenSource();
            //_backgroundTask = Task.Run(BackgroundTaskAsync);
            _isTaskRunning = false;
        }
        public DirWatcherService(string watchDirectory, string magicString, TimeSpan interval, AppDbContext dbContext)
        {
            _watchDirectory = watchDirectory;
            magicString = magicString;
           interval = interval;
            _dbContext = dbContext;
        }
        public bool IsTaskRunning()
        {
            return _isTaskRunning;
        }
        public void StartBackgroundTask(WatchRequest request)
        {
            lock (_lockObj)
            {
                if (!_isTaskRunning)
                {
                    _backgroundTask = Task.Run(() => BackgroundTaskAsync(request, _cancellationTokenSource.Token));
                    _isTaskRunning = true;
                }
            }
        }

        public void StopBackgroundTask()
        {
            lock (_lockObj)
            {
                if (_isTaskRunning)
                {
                    _cancellationTokenSource.Cancel();
                    _backgroundTask.Wait(); // Wait for the task to complete
                    _isTaskRunning = false;
                }
            }
        }

        private async Task BackgroundTaskAsync(WatchRequest request, CancellationToken cancellationToken)
        {
            // Your long-running background task logic goes here
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Background task is running...");
                await Task.Delay(request.Interval, cancellationToken);
            }
        }
        public async Task StartWatching(string directoryPath, TimeSpan interval, string magicString)
        {
            lock (lockObj)
            {
                this.directoryPath = directoryPath;
                this.interval = interval;
                this.magicString = magicString;

                cancellationTokenSource?.Cancel();
                cancellationTokenSource = new CancellationTokenSource();
                task = Task.Run(() => WatchDirectoryAsync(cancellationTokenSource.Token), cancellationTokenSource.Token);
            }
        }

        public async Task StopWatching()
        {
            lock (lockObj)
            {
                cancellationTokenSource?.Cancel();
            }
        }

        private async Task WatchDirectoryAsync(CancellationToken cancellationToken)
        {
            // Implementation for monitoring directory, counting occurrences of magic string, saving results to the database goes here
            while (!cancellationToken.IsCancellationRequested)
            {
                // Implement directory monitoring logic
                await Task.Delay(interval, cancellationToken);
            }
        }
        public async Task<TaskRunDetail> StartAsync()
        {
            var taskRunDetail = new TaskRunDetail
            {
                StartTime = DateTime.Now,
                Status = Models.TaskStatus.InProgress
            };

            _timer = new Timer(async _ =>
            {
                await CheckDirectoryAsync(taskRunDetail);
            }, null, TimeSpan.Zero, interval);

            return taskRunDetail;
        }

        private async Task CheckDirectoryAsync(TaskRunDetail taskRunDetail)
        {
            try
            {
                // Monitor directory for changes
                var currentFiles = Directory.GetFiles(_watchDirectory);
                var dbFiles = await _dbContext.TaskRunDetails
                    .Where(t => t.Id == taskRunDetail.Id)
                    .SelectMany(t => t.FilesAdded)
                    .ToListAsync();

                var newFiles = currentFiles.Except(dbFiles).ToList();
                var deletedFiles = dbFiles.Except(currentFiles).ToList();

                // Count occurrences of magic string in files
                var occurrences = 0;
                foreach (var file in currentFiles)
                {
                    var content = await File.ReadAllTextAsync(file);
                    occurrences += content.Split(magicString).Length - 1;
                }

                // Update task run details
                taskRunDetail.FilesAdded = newFiles;
                taskRunDetail.FilesDeleted = deletedFiles;
                taskRunDetail.TotalOccurrences = occurrences;

                // Save results to the database
                _dbContext.TaskRunDetails.Update(taskRunDetail);
                await _dbContext.SaveChangesAsync();

                taskRunDetail.EndTime = DateTime.Now;
                taskRunDetail.Status = Models.TaskStatus.Success;
            }
            catch (Exception ex)
            {
                taskRunDetail.EndTime = DateTime.Now;
                taskRunDetail.Status = Models.TaskStatus.Failed;
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
        }

        public async Task<TaskRunDetail> StopAsync()
        {
            _timer?.Change(Timeout.Infinite, 0);
            var taskRunDetail = await _dbContext.TaskRunDetails.OrderByDescending(t => t.Id).FirstOrDefaultAsync();
            return taskRunDetail;
        }
    }

}
