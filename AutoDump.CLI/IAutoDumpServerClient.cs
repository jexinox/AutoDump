using Refit;

namespace AutoDump.CLI;

public interface IAutoDumpServerClient
{
    [Get("/api/v1/dumps/metadatas")]
    public Task<UploadedDumpMetadata[]> SearchMetadatas(string locator);

    [Get("/api/v1/dumps/{dumpId}/reports")]
    public Task<Report[]> GetReports(Guid dumpId);
}

public record UploadedDumpMetadata(DumpId DumpId, Locator Locator, string FileName, DateTimeOffset TimeStamp);

public record DumpId(Guid Value);

public record Locator(string Value);

public record Report(
    IReadOnlyList<TypeAndSize> TypesTopBySize,
    IReadOnlyList<TypeAndSize> TypesTopByRetainedSize,
    IReadOnlyList<TypeAndSize> BoxedTypesTopBySize,
    IReadOnlyList<GenerationSize> GenerationsSizes,
    IReadOnlyList<UnhandledException> Exceptions);

public record TypeAndSize(string Type, ulong Size);

public record UnhandledException(int ManagedThreadId, string StackTrace);

public record GenerationSize(Generation Generation, ulong TotalSize, ulong UsedSize);

public enum Generation
{
    Generation0,
    Generation1,
    Generation2,
    Large,
    Pinned,
    Frozen,
    Unknown,
}