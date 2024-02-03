using DirWatcher.Models;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
        
        public DirWatcherService()
        {
            directoryPath = ""; // Default directory path
            interval = TimeSpan.FromMinutes(1); // Default interval
            magicString = ""; // Default magic string
            _cancellationTokenSource = new CancellationTokenSource();
            //_backgroundTask = Task.Run(BackgroundTaskAsync);
            _isTaskRunning = false;
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
            // Your implementation for monitoring directory, counting occurrences of magic string,
            // and saving results to the database goes here
            while (!cancellationToken.IsCancellationRequested)
            {
                // Implement directory monitoring logic
                await Task.Delay(interval, cancellationToken);
            }
        }


    }
}