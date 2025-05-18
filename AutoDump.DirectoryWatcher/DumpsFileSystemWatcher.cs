using System.Threading.Channels;

namespace AutoDump.DirectoryWatcher;

public sealed class DumpsFileSystemWatcher : IDisposable
{
    private readonly FileSystemWatcher _fileSystemWatcher;
    
    private bool _disposed = false;
    
    public DumpsFileSystemWatcher(string directory, string mask)
    {
        _fileSystemWatcher = new(directory, mask);
    }

    public Channel<DumpUploadedEvent> CreateChangesChannel()
    {
        var channel = Channel.CreateUnbounded<DumpUploadedEvent>(new() { SingleWriter = true, SingleReader = true });
        _fileSystemWatcher.Changed += (_, args) => ChannelChanges(args, channel);
        return channel;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }
        
        _disposed = true;
        _fileSystemWatcher.Dispose();
    }

    private static void ChannelChanges(FileSystemEventArgs eventArgs, ChannelWriter<DumpUploadedEvent> writer)
    {
        if (!writer.TryWrite(new(eventArgs.FullPath)))
        {
            Console.WriteLine($"Could not write event about {eventArgs.FullPath} file");
        }
    }
}

public record DumpUploadedEvent(string FullPath);