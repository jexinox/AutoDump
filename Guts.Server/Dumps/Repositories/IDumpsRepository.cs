using Guts.Server.Dumps.FeatureModels;
using Kontur.Results;

namespace Guts.Server.Dumps.Repositories;

public interface IDumpsRepository
{
    Task<Result<RepositoryUploadDumpMetadataError, DumpId>> LoadDumpMetadata(DumpMetadata meta);
}