using Guts.Server.CQRS;
using Guts.Server.DumpsMetadata.FeatureModels;
using Guts.Server.DumpsMetadata.Repositories;
using Guts.Server.DumpsMetadata.Search;
using Guts.Server.DumpsMetadata.Upload;
using Guts.Server.Modules;
using Guts.Server.Reports.FeatureModels;
using Guts.Server.Reports.Repositories;
using Guts.Server.Reports.Search;
using Guts.Server.Reports.Upload;
using Kontur.Results;
using MongoDB.Driver;

namespace Guts.Server.Reports;

public class ReportsModule : IApiModule
{
    public IServiceCollection AddServices(IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddScoped<IMongoCollection<MongoReport>>(
                sp => sp.GetRequiredService<IMongoDatabase>().GetCollection<MongoReport>("Reports"))
            .AddScoped<IReportsRepository, MongoDbReportsRepository>()
            .AddScoped<
                ICommandHandler<UploadReportCommand, Result<UploadReportError>>,
                UploadReportCommandHandler>()
            .AddScoped<
                IQueryHandler<SearchReportsQuery, Result<SearchReportsError, IReadOnlyList<Report>>>,
                SearchReportsQueryHandler>();
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        throw new NotImplementedException();
    }
}