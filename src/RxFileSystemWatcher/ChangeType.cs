using System.IO;

namespace RxFileSystemWatcher
{
    public enum ChangeType
    {
        Created,
        Changed,
        Renamed,
        Deleted,
        DiscoveredFromPolling
    }
}
