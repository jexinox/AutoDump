using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace AutoDump.DirectoryWatcher;

public sealed class DumpsFileSystemWatcher : IDisposable
{
    private readonly ILogger<DumpsFileSystemWatcher> _logger;
    private readonly FileSystemWatcher _fileSystemWatcher;
    
    private bool _disposed = false;
    
    public DumpsFileSystemWatcher(string directory, string mask, ILogger<DumpsFileSystemWatcher> logger)
    {
        _logger = logger;
        _logger.LogInformation("Started watching directory {directory}", directory);
        _fileSystemWatcher = new(directory, mask);
    }

    public Channel<DumpUploadedEvent> CreateChangesChannel()
    {
        var channel = Channel.CreateUnbounded<DumpUploadedEvent>(new() { SingleWriter = true, SingleReader = true });
        _fileSystemWatcher.Created += (_, args) => ChannelChanges(args, channel, _logger);
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

    private static void ChannelChanges(
        FileSystemEventArgs eventArgs,
        ChannelWriter<DumpUploadedEvent> writer,
        ILogger<DumpsFileSystemWatcher> logger)
    {
        logger.LogInformation("Found new dump: {dumpName}", eventArgs.Name);
        if (!writer.TryWrite(new(eventArgs.FullPath)))
        {
            logger.LogError("Could not write event about {fullPath} file", eventArgs.FullPath);
        }
    }
}

public record DumpUploadedEvent(string FullPath);