using Guts.Server.DumpsMetadata.FeatureModels;
using Kontur.Results;

namespace Guts.Server.DumpsMetadata.Repositories;

public interface IDumpsMetadataRepository
{
    Task<Result<RepositoryUploadDumpMetadataError, DumpId>> LoadDumpMetadata(DumpMetadata meta);
    
    Task<Result<RepositorySearchDumpMetadataError, IReadOnlyList<UploadedDumpMetadata>>> Search(Locator locator);
}

public record RepositoryUploadDumpMetadataError;

public record RepositorySearchDumpMetadataError;
