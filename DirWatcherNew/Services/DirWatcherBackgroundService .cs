using DirWatcher.Controllers;
using DirWatcher.Models;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using DirWatcherNew.Models;

namespace DirWatcherNew.Services
{
    
    public class DirWatcherBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly AppDbContext _dbContext;
        private readonly IOptionsMonitor<ConfigurationModel> _configOptionsMonitor;
        private readonly ILogger<DirWatcherBackgroundService> _logger;
        private readonly string _watchedDirectoryPath;
        public DirWatcherBackgroundService(IServiceScopeFactory scopeFactory, AppDbContext dbContext, IOptionsMonitor<ConfigurationModel> configOptionsMonitor, ILogger<DirWatcherBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _dbContext = dbContext;
            _configOptionsMonitor = configOptionsMonitor;
            _logger = logger;
            _watchedDirectoryPath = "D:\\";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var taskRunDetail = new TaskRunDetail
                {
                    StartTime = DateTime.UtcNow,
                    Status = DirWatcher.Models.TaskStatus.InProgress
                };

                // monitoring the directory and saving results to the database goes here

                taskRunDetail.EndTime = DateTime.UtcNow;
                taskRunDetail.TotalRuntime = taskRunDetail.EndTime - taskRunDetail.StartTime;
                taskRunDetail.Status = DirWatcher.Models.TaskStatus.Success;

                _dbContext.TaskRunDetails.Add(taskRunDetail);
                await _dbContext.SaveChangesAsync();

                await Task.Delay(TimeSpan.FromMinutes(_configOptionsMonitor.CurrentValue.TimeIntervalMinutes), stoppingToken);
            }
            
        }
       

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            // Handling file change event
            _logger.LogInformation($"File: {e.FullPath} {e.ChangeType}");

            
            SaveToDatabase(e);
        }

        private void SaveToDatabase(FileSystemEventArgs e)
        {
            // save the file information to the database using Entity Framework 
            

            using (var dbContext = new AppDbContext())
            {
                dbContext.FileChanges.Add(new FileChange { FilePath = e.FullPath, ChangeType = e.ChangeType.ToString(), Timestamp = DateTime.Now });
                dbContext.SaveChanges();
            }
        }
    }
}
