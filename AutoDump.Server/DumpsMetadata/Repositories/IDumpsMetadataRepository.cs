using AutoDump.Server.DumpsMetadata.FeatureModels;
using Kontur.Results;

namespace AutoDump.Server.DumpsMetadata.Repositories;

public interface IDumpsMetadataRepository
{
    Task<Result<RepositoryUploadDumpMetadataError, DumpId>> LoadDumpMetadata(DumpMetadata meta);
    
    Task<Result<RepositorySearchDumpMetadataError, IReadOnlyList<UploadedDumpMetadata>>> Search(Locator locator);
}

public record RepositoryUploadDumpMetadataError;

public record RepositorySearchDumpMetadataError;
