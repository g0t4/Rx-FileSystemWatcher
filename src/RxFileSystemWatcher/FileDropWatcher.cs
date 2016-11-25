using System.Reactive.Subjects;
using System.Security.Cryptography.X509Certificates;

namespace RxFileSystemWatcher
{
	using System;
	using System.IO;
	using System.Reactive.Linq;
	using System.Reactive;

    public interface IFileDropWatcher
    {
        IObservable<FileDropped> Dropped { get; }
        void Start();
        void Stop();
        void PollExisting();
    }

    /// <summary>
	///     An observable abstraction to monitor for files dropped into a directory
	/// </summary>
	public class FileDropWatcher : IDisposable, IFileDropWatcher
    {
		private readonly string _Path;
		private readonly string _Filter;
	    private readonly bool _includeSubDirs;
	    private readonly ObservableFileSystemWatcher _Watcher;
		private readonly Subject<FileDropped> _PollResults;

		public IObservable<FileDropped> Dropped { get; private set; }

        public FileDropWatcher(string path, string filter) : this(path, filter, false)
        {
        }

        public FileDropWatcher(string path, string filter, bool includeSubDirs)
		{
			_Path = path;
			_Filter = filter;
		    _includeSubDirs = includeSubDirs;
		    _PollResults = new Subject<FileDropped>();
            _Watcher = new ObservableFileSystemWatcher(w =>
			{
				w.Path = path;
				w.Filter = filter;
				// note: filtering on changes can help reduce excessive notifications, make sure to verify any changes with integration tests
				w.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
			    w.IncludeSubdirectories = includeSubDirs;
			});

			var renames = _Watcher.Renamed.Select(r => new FileDropped(r));
			var creates = _Watcher.Created.Select(c => new FileDropped(c));
			var changed = _Watcher.Changed.Select(c => new FileDropped(c));

			Dropped = creates
				.Merge(renames)
				.Merge(changed)
				.Merge(_PollResults);
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

		public void PollExisting()
		{
            foreach (var existingFile in Directory.GetFiles(_Path, _Filter, SearchOption))
            {
                _PollResults.OnNext(new FileDropped(existingFile));
            }
        }

	    public SearchOption SearchOption
	    {
	        get { return _includeSubDirs?SearchOption.AllDirectories : SearchOption.TopDirectoryOnly; }
	    }
	}
}