using Refit;

namespace AutoDump.DumpsDaemon;

public interface IAutoDumpServerClient
{
    [Post("/api/v1/dumps/{dumpId}/reports")]
    Task UploadReport(Guid dumpId, Report report);

    [Get("/api/v1/dumps/{dumpId}")]
    Task<Stream> GetDump(Guid dumpId);
}

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