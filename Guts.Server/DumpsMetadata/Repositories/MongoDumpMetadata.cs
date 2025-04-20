using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Guts.Server.DumpsMetadata.Repositories;

[BsonIgnoreExtraElements]
public class MongoDumpMetadata
{
    public required Guid DumpId { get; set; }

    public required string Locator { get; set; } = default!;

    public required string FileName { get; set; } = default!;

    [BsonRepresentation(BsonType.Document)]
    public required DateTimeOffset TimeStamp { get; set; } = default;
}