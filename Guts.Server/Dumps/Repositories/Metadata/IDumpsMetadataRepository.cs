using Guts.Server.Dumps.FeatureModels;
using Kontur.Results;

namespace Guts.Server.Dumps.Repositories.Metadata;

public interface IDumpsMetadataRepository
{
    Task<Result<RepositoryUploadDumpMetadataError, DumpId>> LoadDumpMetadata(DumpMetadata meta);
    
    Task<Result<RepositorySearchDumpMetadataError, IReadOnlyList<UploadedDumpMetadata>>> Search(Locator locator);
}

public record RepositoryUploadDumpMetadataError;

public record RepositorySearchDumpMetadataError;
