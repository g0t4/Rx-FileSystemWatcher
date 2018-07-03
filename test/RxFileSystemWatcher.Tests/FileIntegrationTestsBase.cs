using System;
using System.IO;
using Xunit;

namespace RxFileSystemWatcher.Tests
{

    public class FileIntegrationTestsBase : IDisposable
    {
        protected string TempPath;

        public FileIntegrationTestsBase()
        {
            TempPath = Guid.NewGuid().ToString();
            Directory.CreateDirectory(TempPath);
        }

        public void Dispose()
        {
            if (!Directory.Exists(TempPath))
            {
                return;
            }
            Directory.Delete(TempPath, true);
        }
    }
}