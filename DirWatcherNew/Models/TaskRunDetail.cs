using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DirWatcher.Models
{
    public class TaskRunDetail
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public TimeSpan? TotalRuntime { get; set; }
        public List<string> FilesAdded { get; set; }
        public List<string> FilesDeleted { get; set; }
        public int TotalOccurrences { get; set; }
        public TaskStatus Status { get; set; }
    }

    public enum TaskStatus
    {
        InProgress,
        Success,
        Failed
    }
}
