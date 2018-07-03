using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Xunit;
using RxFileSystemWatcher;

namespace RxFileSystemWatcher.Tests
{
    public class ObservableFileSystemWatcherTests : FileIntegrationTestsBase
    {
        [Fact]
        public async Task WriteToFile_StreamsChanged()
        {
            using (var watcher = new ObservableFileSystemWatcher(c => { c.Path = TempPath; }))
            {
                var firstChanged = watcher.Changed.FirstAsync().ToTask();
                watcher.Start();

                File.WriteAllText(Path.Combine(TempPath, "Changed.Txt"), "foo");

                var changed = await firstChanged;
                Assert.Equal(WatcherChangeTypes.Changed, changed.ChangeType);
                Assert.Equal("Changed.Txt", changed.Name);
            }
        }

        [Fact]
        public async Task CreateFile_StreamsCreated()
        {
            using (var watcher = new ObservableFileSystemWatcher(c => { c.Path = TempPath; }))
            {
                var firstCreated = watcher.Created.FirstAsync().ToTask();
                var filePath = Path.Combine(TempPath, "Created.Txt");
                watcher.Start();

                File.WriteAllText(filePath, "foo");

                var created = await firstCreated;
                Assert.Equal(WatcherChangeTypes.Created, created.ChangeType);
                Assert.Equal("Created.Txt", created.Name);
            }
        }

        [Fact]
        public async Task DeleteFile_StreamsDeleted()
        {
            using (var watcher = new ObservableFileSystemWatcher(c => { c.Path = TempPath; }))
            {
                var firstDeleted = watcher.Deleted.FirstAsync().ToTask();
                var filePath = Path.Combine(TempPath, "ToDelete.Txt");
                File.WriteAllText(filePath, "foo");
                watcher.Start();

                File.Delete(filePath);

                var deleted = await firstDeleted;
                Assert.Equal(WatcherChangeTypes.Deleted, deleted.ChangeType);
                Assert.Equal("ToDelete.Txt", deleted.Name);
            }
        }

        [Fact]
        public async Task DeleteMonitoredDirectory_StreamsError()
        {
            using (var watcher = new ObservableFileSystemWatcher(c => { c.Path = TempPath; }))
            {
                var firstError = watcher.Errors.FirstAsync().ToTask();
                watcher.Start();

                Directory.Delete(TempPath);

                var error = await firstError;
                Assert.Equal("Access is denied", error.GetException().Message);
            }
        }

        [Fact]
        public async Task RenameFile_StreamsRenamed()
        {
            using (var watcher = new ObservableFileSystemWatcher(c => { c.Path = TempPath; }))
            {
                var firstRenamed = watcher.Renamed.FirstAsync().ToTask();
                var originalPath = Path.Combine(TempPath, "Changed.Txt");
                File.WriteAllText(originalPath, "foo");
                watcher.Start();

                var renamedPath = Path.Combine(TempPath, "Renamed.Txt");
                File.Move(originalPath, renamedPath);

                var renamed = await firstRenamed;
                Assert.Equal(renamed.OldFullPath, originalPath);
                Assert.Equal(renamed.FullPath, renamedPath);
            }
        }
    }
}