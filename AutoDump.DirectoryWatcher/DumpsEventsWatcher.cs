using System.Threading.Channels;

namespace AutoDump.DirectoryWatcher;

public class DumpsEventsWatcher
{
    private readonly ChannelReader<DumpUploadedEvent> _reader;
    private readonly IAutoDumpServerClient _serverClient;

    public DumpsEventsWatcher(ChannelReader<DumpUploadedEvent> reader, IAutoDumpServerClient serverClient)
    {
        _reader = reader;
        _serverClient = serverClient;
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
                var dumpId = await _serverClient.UploadMetaData(new("test_app", fileInfo.Name, fileInfo.LastWriteTimeUtc));

                await using var dump = File.OpenRead(dumpUploadedEvent.FullPath);
                await _serverClient.UploadDump(dumpId.DumpId, dump);
            }
        });
    }
}