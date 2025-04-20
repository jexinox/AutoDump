using Guts.Server.Dumps.FeatureModels;
using Kontur.Results;
using MongoDB.Driver;

namespace Guts.Server.Dumps.Repositories.Metadata;

public class MongoDbMetadataRepository(
    IMongoCollection<MongoDumpMetadata> collection) : IDumpsMetadataRepository
{
    public async Task<Result<RepositoryUploadDumpMetadataError, DumpId>> LoadDumpMetadata(DumpMetadata meta)
    {
        var dumpId = Guid.NewGuid();

        var mongoMeta = new MongoDumpMetadata
        {
            DumpId = dumpId,
            Locator = meta.Locator,
            FileName = meta.FileName,
            TimeStamp = meta.TimeStamp
        };

        await collection.InsertOneAsync(mongoMeta);
        
        return new DumpId(dumpId);
    }
}