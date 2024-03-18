using Guts.Models;
using Kontur.Results;

namespace Guts.Server.Dumps.Repositories.Mongo;

/*
 * (
    IMongoCollection<MongoDumpMetadata> collection,
    IGridFSBucket<Guid> gridFsBucket)
 */
public class MongoDbDumpsRepository : IDumpsRepository
{
    public Task<Result<DbUploadDumpError>> LoadDump(DumpMetadata hostName, DumpArchive dumpArchive)
    {
        throw new NotImplementedException();
    }
}