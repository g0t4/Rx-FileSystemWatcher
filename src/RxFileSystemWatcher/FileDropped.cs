namespace RxFileSystemWatcher
{
	using System.IO;

	public class FileDropped
	{
		public FileDropped(FileSystemEventArgs fileEvent)
		{
			Name = fileEvent.Name;
			FullPath = fileEvent.FullPath;
		}

		public string Name { get; set; }
		public string FullPath { get; set; }
	}
}