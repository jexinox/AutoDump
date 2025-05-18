namespace AutoDump.Server.Reports.FeatureModels;

public record Report(
    DumpId DumpId,
    IReadOnlyList<TypeAndSize> TypesTopBySize,
    IReadOnlyList<TypeAndSize> TypesTopByRetainedSize,
    IReadOnlyList<TypeAndSize> BoxedTypesTopBySize,
    IReadOnlyList<GenerationSize> GenerationsSizes,
    IReadOnlyList<UnhandledException> Exceptions);

public record DumpId(Guid Value);

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