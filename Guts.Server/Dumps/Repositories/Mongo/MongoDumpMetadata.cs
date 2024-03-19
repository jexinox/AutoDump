using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Guts.Server.Dumps.Repositories.Mongo;

[BsonIgnoreExtraElements]
public class MongoDumpMetadata
{
    public required Guid BlobStorageFileId { get; set; }

    public required string HostName { get; set; } = default!;

    public required string FileName { get; set; } = default!;

    [BsonRepresentation(BsonType.Document)]
    public required DateTimeOffset TimeStamp { get; set; } = default;
}