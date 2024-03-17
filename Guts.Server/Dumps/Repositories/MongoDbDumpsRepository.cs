using Guts.Models;
using Kontur.Results;

namespace Guts.Server.Dumps.Repositories;

public class MongoDbDumpsRepository : IDumpsRepository
{
    public Task<Result<DbUploadDumpError>> LoadDump(HostName hostName, DumpArchive dumpArchive)
    {
        throw new NotImplementedException();
    }
}