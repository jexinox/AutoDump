using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Guts.Server.Dumps.Repositories.Mongo;

[BsonIgnoreExtraElements]
public class MongoDumpMetadata
{
    public Guid BlobStorageFileId { get; set; }

    public string HostName { get; set; } = default!;

    public string FileName { get; set; } = default!;

    [BsonRepresentation(BsonType.DateTime)]
    public DateTimeOffset TimeStamp { get; set; } = default;
}