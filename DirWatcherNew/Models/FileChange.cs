namespace DirWatcherNew.Models
{
    public class FileChange
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public string ChangeType { get; set; }
        public DateTime Timestamp { get; set; }
    }

}
