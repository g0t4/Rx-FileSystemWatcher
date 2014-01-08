namespace RxFileSystemWatcher
{
	using System;
	using System.IO;
	using System.Reactive.Linq;

	/// <summary>
	///     An observable abstraction to monitor for files dropped into a directory
	/// </summary>
	public class FileDropWatcher : IDisposable
	{
		private readonly ObservableFileSystemWatcher _Watcher;
		public IObservable<FileDropped> Dropped { get; private set; }

		public FileDropWatcher(string path, string filter)
		{
			_Watcher = new ObservableFileSystemWatcher(w =>
			{
				w.Path = path;
				w.Filter = filter;
				// note: filtering on changes can help reduce excessive notifications, make sure to verify any changes with integration tests
				w.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
			});

			var renames = _Watcher.Renamed.Select(r => new FileDropped(r));
			var creates = _Watcher.Created.Select(c => new FileDropped(c));
			var changed = _Watcher.Changed.Select(c => new FileDropped(c));

			Dropped = creates
				.Merge(renames)
				.Merge(changed);
		}

		public void Start()
		{
			_Watcher.Start();
		}

		public void Stop()
		{
			_Watcher.Stop();
		}

		public void Dispose()
		{
			_Watcher.Dispose();
		}
	}
}