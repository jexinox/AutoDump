using AutoDump.Server.DumpsMetadata.FeatureModels;
using Kontur.Results;
using MongoDB.Driver;

namespace AutoDump.Server.DumpsMetadata.Repositories;

public class MongoDbMetadataRepository(
    IMongoCollection<MongoDumpMetadata> collection) : IDumpsMetadataRepository
{
    public async Task<Result<RepositoryUploadDumpMetadataError, DumpId>> LoadDumpMetadata(DumpMetadata meta)
    {
        var dumpId = Guid.NewGuid();

        var mongoMeta = new MongoDumpMetadata
        {
            DumpId = dumpId,
            Locator = meta.Locator.Value,
            FileName = meta.FileName,
            TimeStamp = meta.TimeStamp
        };

        await collection.InsertOneAsync(mongoMeta);
        
        return new DumpId(dumpId);
    }

    public async Task<Result<RepositorySearchDumpMetadataError, IReadOnlyList<UploadedDumpMetadata>>> Search(Locator locator)
    {
        var searchResult = await collection.Find(meta => meta.Locator == locator.Value).ToListAsync();

        return searchResult.Select(ToMetadata).ToArray();
    }

    private static UploadedDumpMetadata ToMetadata(MongoDumpMetadata meta)
    {
        return new(new(meta.DumpId), new(meta.Locator), meta.FileName, meta.TimeStamp);
    }
}