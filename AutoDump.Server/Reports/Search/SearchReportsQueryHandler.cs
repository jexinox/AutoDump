using AutoDump.Server.CQRS;
using AutoDump.Server.Reports.FeatureModels;
using AutoDump.Server.Reports.Repositories;
using Kontur.Results;

namespace AutoDump.Server.Reports.Search;

public class SearchReportsQueryHandler(IReportsRepository repository) : 
    IQueryHandler<SearchReportsQuery, Result<SearchReportsError, IReadOnlyList<Report>>>
{
    public async Task<Result<SearchReportsError, IReadOnlyList<Report>>> Handle(SearchReportsQuery command)
    {
        return await repository
            .Search(command.DumpId)
            .MapFault(dbError => new SearchReportsError());
    }
}

public record SearchReportsError;