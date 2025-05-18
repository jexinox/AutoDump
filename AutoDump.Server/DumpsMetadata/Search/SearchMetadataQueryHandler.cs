using AutoDump.Server.CQRS;
using AutoDump.Server.DumpsMetadata.FeatureModels;
using AutoDump.Server.DumpsMetadata.Repositories;
using Kontur.Results;

namespace AutoDump.Server.DumpsMetadata.Search;

public class SearchMetadataQueryHandler(IDumpsMetadataRepository repository) 
    : IQueryHandler<SearchMetadataQuery, Result<SearchDumpMetadataError, IReadOnlyList<UploadedDumpMetadata>>>
{
    public async Task<Result<SearchDumpMetadataError, IReadOnlyList<UploadedDumpMetadata>>> Handle(SearchMetadataQuery command)
    {
        return await repository
            .Search(command.Locator)
            .MapFault(dbError => new SearchDumpMetadataError());
    }
}

public record SearchDumpMetadataError;