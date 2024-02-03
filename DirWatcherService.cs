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

        public DirWatcherService()
        {
            directoryPath = ""; // Default directory path
            interval = TimeSpan.FromMinutes(1); // Default interval
            magicString = ""; // Default magic string
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