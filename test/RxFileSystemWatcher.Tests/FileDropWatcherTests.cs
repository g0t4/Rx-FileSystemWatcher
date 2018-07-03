using System.IO;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Xunit;
using RxFileSystemWatcher;

namespace RxFileSystemWatcher.Tests
{

    public class FileDropWatcherTests : FileIntegrationTestsBase
    {
        [Fact]
        public async Task FileDropped_NoExistingFile_StreamsDropped()
        {
            using (var watcher = new FileDropWatcher(TempPath, "Monitored.Txt"))
            {
                var firstDropped = watcher.Dropped.FirstAsync().ToTask();
                watcher.Start();

                var monitoredFile = Path.Combine(TempPath, "Monitored.Txt");
                File.WriteAllText(monitoredFile, "foo");

                var dropped = await firstDropped;
                Assert.Equal(dropped.Name, "Monitored.Txt");
                Assert.Equal(dropped.FullPath, monitoredFile);
            }
        }

        [Fact]
        public async Task FileRenamed_NoExistingFile_StreamsDropped()
        {
            using (var watcher = new FileDropWatcher(TempPath, "Monitored.Txt"))
            {
                var firstDropped = watcher.Dropped.FirstAsync().ToTask();
                var otherFile = Path.Combine(TempPath, "Other.Txt");
                File.WriteAllText(otherFile, "foo");
                watcher.Start();

                var monitoredFile = Path.Combine(TempPath, "Monitored.Txt");
                File.Move(otherFile, monitoredFile);

                var dropped = await firstDropped;
                Assert.Equal(dropped.Name, "Monitored.Txt");
                Assert.Equal(dropped.FullPath, monitoredFile);
            }
        }

        [Fact]
        public async Task Overwrite_ExistingFile_StreamsDropped()
        {
            using (var watcher = new FileDropWatcher(TempPath, "Monitored.Txt"))
            {
                var firstDropped = watcher.Dropped.FirstAsync().ToTask();
                var monitoredFile = Path.Combine(TempPath, "Monitored.Txt");
                File.WriteAllText(monitoredFile, "foo");
                watcher.Start();

                File.WriteAllText(monitoredFile, "bar");

                var dropped = await firstDropped;
                Assert.Equal(dropped.Name, "Monitored.Txt");
                Assert.Equal(dropped.FullPath, monitoredFile);
            }
        }

        [Fact]
        public async Task PollExisting_FileBeforeStart_StreamsDropped()
        {
            using (var watcher = new FileDropWatcher(TempPath, "Monitored.Txt"))
            {
                var firstDropped = watcher.Dropped.FirstAsync().ToTask();
                var monitoredFile = Path.Combine(TempPath, "Monitored.Txt");
                File.WriteAllText(monitoredFile, "foo");

                watcher.PollExisting();

                var dropped = await firstDropped;
                Assert.Equal(dropped.Name, "Monitored.Txt");
                Assert.Equal(dropped.FullPath, monitoredFile);
            }
        }

        [Fact]
        public async Task PollExisting_SecondTime_StreamsSecondTime()
        {
            using (var watcher = new FileDropWatcher(TempPath, "Monitored.Txt"))
            {
                var secondDropped = watcher.Dropped.Skip(1).FirstAsync().ToTask();
                var monitoredFile = Path.Combine(TempPath, "Monitored.Txt");
                File.WriteAllText(monitoredFile, "foo");

                watcher.PollExisting();
                watcher.PollExisting();

                var dropped = await secondDropped;
                Assert.Equal(dropped.Name, "Monitored.Txt");
                Assert.Equal(dropped.FullPath, monitoredFile);
            }
        }
    }
}