using Guts.Server.CQRS;
using Guts.Server.Modules;
using Guts.Server.Reports.FeatureModels;
using Guts.Server.Reports.Repositories;
using Guts.Server.Reports.Search;
using Guts.Server.Reports.Upload;
using Kontur.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
        endpoints
            .MapPost(ApiConstants.V1 + "/dumps/{dumpId:guid}/reports", UploadReport)
            .WithName(nameof(UploadReport));
        
        endpoints
            .MapGet(ApiConstants.V1 + "/dumps/{dumpId:guid}/reports", SearchReports)
            .WithName(nameof(SearchReports));
       
        return endpoints;
    }
    
    private static async Task<Results<Ok<IReadOnlyList<Report>>, BadRequest>> SearchReports(
        [FromRoute] Guid dumpId,
        [FromServices] IQueryHandler<SearchReportsQuery, Result<SearchReportsError, IReadOnlyList<Report>>> handler)
    {
        var handleResult = await handler
            .Handle(new(new(dumpId)))
            .MapValue(reports => TypedResults.Ok(reports))
            .MapFault(_ => TypedResults.BadRequest());
                    
        return handleResult.TryGetValue(out var value, out var fault) 
            ? value 
            : fault;
    }
    
    private static async Task<Results<CreatedAtRoute, BadRequest>> UploadReport(
        [FromRoute] Guid dumpId,
        [FromBody] UploadReportRequest request,
        [FromServices] ICommandHandler<UploadReportCommand, Result<UploadReportError>> handler)
    {
        var handleResult = await handler
            .Handle(new(ToReport(dumpId, request.Report)))
            .MapFault(_ => TypedResults.BadRequest());
                    
        return handleResult.TryGetFault(out var fault) 
            ? fault
            : TypedResults.CreatedAtRoute(nameof(SearchReports), new()
            {
                ["dumpId"] = dumpId,
            });
    }

    private static Report ToReport(Guid dumpId, PresentationReport request)
    {
        return new(
            new(dumpId),
            request.TypesTopBySize.Select(x => new TypeAndSize(x.Type, x.Size)).ToList(),
            request.TypesTopByRetainedSize.Select(x => new TypeAndSize(x.Type, x.Size)).ToList(),
            request.BoxedTypesTopBySize.Select(x => new TypeAndSize(x.Type, x.Size)).ToList(),
            request.GenerationsSizes.Select(x => new GenerationSize(ToGeneration(x.Generation), x.TotalSize, x.UsedSize)).ToList(),
            request.Exceptions.Select(x => new UnhandledException(x.ManagedThreadId, x.StackTrace)).ToList());
    }

    private static Generation ToGeneration(PresentationGeneration generation)
    {
        return generation switch
        {
            PresentationGeneration.Generation0 => Generation.Generation0,
            PresentationGeneration.Generation1 => Generation.Generation1,
            PresentationGeneration.Generation2 => Generation.Generation2,
            PresentationGeneration.Large => Generation.Large,
            PresentationGeneration.Pinned => Generation.Pinned,
            PresentationGeneration.Frozen => Generation.Frozen,
            PresentationGeneration.Unknown => Generation.Unknown,
            _ => throw new ArgumentOutOfRangeException(nameof(generation), generation, null)
        };
    }
    
    private record UploadReportRequest(PresentationReport Report);
    
    private record PresentationReport(
        IReadOnlyList<PresentationTypeAndSize> TypesTopBySize,
        IReadOnlyList<PresentationTypeAndSize> TypesTopByRetainedSize,
        IReadOnlyList<PresentationTypeAndSize> BoxedTypesTopBySize,
        IReadOnlyList<PresentationGenerationSize> GenerationsSizes,
        IReadOnlyList<PresentationUnhandledException> Exceptions);

    private record PresentationTypeAndSize(string Type, ulong Size);

    private record PresentationUnhandledException(int ManagedThreadId, string StackTrace);

    private record PresentationGenerationSize(PresentationGeneration Generation, ulong TotalSize, ulong UsedSize);

    private enum PresentationGeneration
    {
        Generation0,
        Generation1,
        Generation2,
        Large,
        Pinned,
        Frozen,
        Unknown,
    }
}