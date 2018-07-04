using System.IO;

namespace RxFileSystemWatcher
{
    public class FileDropped
    {
        public FileDropped()
        {
        }

        public FileDropped(FileSystemEventArgs fileEvent)
        {
            Name = fileEvent.Name;
            FullPath = fileEvent.FullPath;
        }

        public FileDropped(string filePath)
        {
            Name = Path.GetFileName(filePath);
            FullPath = filePath;
        }

        public string Name { get; set; }
        public string FullPath { get; set; }
        public ChangeType DroppedType { get; set; }
    }
}
