using System;

namespace DirWatcher.Models
{
    public class WatchRequest
    {
        public string DirectoryPath { get; set; }
        public TimeSpan Interval { get; set; }
        public string MagicString { get; set; }
    }
}