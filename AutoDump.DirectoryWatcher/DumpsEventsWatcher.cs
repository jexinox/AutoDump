using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace AutoDump.DirectoryWatcher;

public class DumpsEventsWatcher
{
    private readonly ChannelReader<DumpUploadedEvent> _reader;
    private readonly IAutoDumpServerClient _serverClient;
    private readonly ILogger<DumpsEventsWatcher> _logger;

    public DumpsEventsWatcher(
        ChannelReader<DumpUploadedEvent> reader,
        IAutoDumpServerClient serverClient,
        ILogger<DumpsEventsWatcher> logger)
    {
        _reader = reader;
        _serverClient = serverClient;
        _logger = logger;
    }

    public Task Start()
    {
        return Task.Run(async () =>
        {
            while (true)
            {
                if (!_reader.TryRead(out var dumpUploadedEvent))
                {
                    continue;
                }

                var fileInfo = new FileInfo(dumpUploadedEvent.FullPath);

                _logger.LogInformation(
                    "Started uploading dump. Locator: {locator}, Filename: {fileName}",
                    fileInfo.DirectoryName,
                    fileInfo.Name);
                var dumpId = await _serverClient.UploadMetaData(
                    new(fileInfo.DirectoryName!, fileInfo.Name, fileInfo.LastWriteTimeUtc));

                await using var dump = File.OpenRead(dumpUploadedEvent.FullPath);
                await _serverClient.UploadDump(dumpId.DumpId, dump);
                _logger.LogInformation("Successfully uploaded dump {fileName}", fileInfo.Name);
            }
        });
    }
}