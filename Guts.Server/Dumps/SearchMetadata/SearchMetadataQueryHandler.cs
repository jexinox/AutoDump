using Guts.Server.CQRS;
using Guts.Server.Dumps.FeatureModels;
using Guts.Server.Dumps.Repositories.Metadata;
using Kontur.Results;

namespace Guts.Server.Dumps.SearchMetadata;

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