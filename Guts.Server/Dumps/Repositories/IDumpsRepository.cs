using Guts.Models;
using Kontur.Results;

namespace Guts.Server.Dumps.Repositories;

public interface IDumpsRepository
{
    Task<Result<DbUploadDumpError>> LoadDump(DumpMetadata hostName, DumpArchive dumpArchive);
}