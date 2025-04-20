using Guts.Server.Dumps.FeatureModels;
using Kontur.Results;

namespace Guts.Server.Dumps.Repositories;

public interface IDumpsRepository
{
    Task<Result<RepositoryUploadDumpError>> LoadDump(DumpId id, Dump dump);
}

public record RepositoryUploadDumpError;
