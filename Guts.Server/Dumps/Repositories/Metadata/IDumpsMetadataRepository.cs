using Guts.Server.Dumps.FeatureModels;
using Guts.Server.Dumps.Repositories.Metadata;
using Kontur.Results;

namespace Guts.Server.Dumps.Repositories;

public interface IDumpsMetadataRepository
{
    Task<Result<RepositoryUploadDumpMetadataError, DumpId>> LoadDumpMetadata(DumpMetadata meta);
}