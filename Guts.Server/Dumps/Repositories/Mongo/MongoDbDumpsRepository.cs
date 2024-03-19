using Guts.Server.Dumps.FeatureModels;
using Kontur.Results;
using MongoDB.Driver;

namespace Guts.Server.Dumps.Repositories.Mongo;

public class MongoDbDumpsRepository(
    IMongoCollection<MongoDumpMetadata> collection) : IDumpsRepository
{
    public async Task<Result<RepositoryUploadDumpMetadataError, DumpId>> LoadDumpMetadata(DumpMetadata meta)
    {
        var dumpId = Guid.NewGuid();

        var mongoMeta = new MongoDumpMetadata
        {
            BlobStorageFileId = dumpId,
            HostName = meta.HostName,
            FileName = meta.FileName,
            TimeStamp = meta.TimeStamp
        };

        await collection.InsertOneAsync(mongoMeta);
        
        return new DumpId(dumpId);
    }
}