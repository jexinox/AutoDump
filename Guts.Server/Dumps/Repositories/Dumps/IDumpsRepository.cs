using Guts.Server.Dumps.FeatureModels;
using Guts.Server.Dumps.Repositories.Metadata;
using Kontur.Results;

namespace Guts.Server.Dumps.Repositories.Dumps;

public interface IDumpsRepository
{
    Task<Result<RepositoryUploadDumpMetadataError>> LoadDump(DumpId id, Dump dump);
}