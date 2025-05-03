using Refit;

namespace Guts.DirectoryWatcher;

public interface IGutsServerClient
{
    [Put("/api/v1/dumps/metadatas")]
    Task<UploadDumpMetadataResponse> UploadMetaData(UploadDumpMetadataRequest request);

    [Put("/api/v1/dumps/{dumpId}")]
    Task UploadDump(Guid dumpId, Stream dump);
}

public record UploadDumpMetadataResponse(Guid DumpId);

public record UploadDumpMetadataRequest(string Locator, string FileName, DateTimeOffset TimeStamp);
