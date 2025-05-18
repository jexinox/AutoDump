using MongoDB.Bson.Serialization.Attributes;

namespace AutoDump.Server.Reports.Repositories;

[BsonIgnoreExtraElements]
public class MongoReport
{
    public required Guid DumpId { get; init; }
    
    public required MongoTypeAndSize[] TypesTopBySize { get; init; }
        
    public required MongoTypeAndSize[] TypesTopByRetainedSize { get; init; }
    
    public required MongoTypeAndSize[] BoxedTypesTopBySize { get; init; }
    
    public required MongoGenerationSize[] GenerationsSizes { get; init; }
    
    public required MongoUnhandledException[] Exceptions { get; init; }
}

public record MongoTypeAndSize(string Type, ulong Size);

public record MongoUnhandledException(int ManagedThreadId, string StackTrace);

public record MongoGenerationSize(MongoGeneration Generation, ulong TotalSize, ulong UsedSize);

public enum MongoGeneration
{
    Generation0,
    Generation1,
    Generation2,
    Large,
    Pinned,
    Frozen,
    Unknown,
}