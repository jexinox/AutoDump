using Guts.Server.CQRS;
using Guts.Server.Dumps;
using Guts.Server.DumpsMetadata.FeatureModels;
using Guts.Server.DumpsMetadata.Repositories;
using Guts.Server.DumpsMetadata.Search;
using Guts.Server.DumpsMetadata.Upload;
using Guts.Server.Modules;
using Kontur.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Guts.Server.DumpsMetadata;

public class DumpsMetadataModule : IApiModule
{
    public IServiceCollection AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<IMongoCollection<MongoDumpMetadata>>(
                sp => sp.GetRequiredService<IMongoDatabase>().GetCollection<MongoDumpMetadata>("DumpsMeta"))
            .AddScoped<IDumpsMetadataRepository, MongoDbMetadataRepository>()
            .AddScoped<
                ICommandHandler<UploadDumpMetadataCommand, Result<UploadDumpMetadataError, DumpId>>,
                UploadDumpMetadataHandler>()
            .AddScoped<
                IQueryHandler<SearchMetadataQuery, Result<SearchDumpMetadataError, IReadOnlyList<UploadedDumpMetadata>>>,
                SearchMetadataQueryHandler>();
        
        return serviceCollection;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapPost(ApiConstants.V1 + "/dumps/metadatas", UploadDumpMetadata)
            .WithName(nameof(UploadDumpMetadata));
        
        endpoints
            .MapGet(ApiConstants.V1 + "/dumps/metadatas", SearchDumpMetadata)
            .WithName(nameof(SearchDumpMetadata));
       
        return endpoints;
    }
    
    private record UploadDumpMetadataRequest(string Locator, string FileName, DateTimeOffset TimeStamp);
    
    private record UploadDumpMetadataResponse(Guid DumpId);

    private static async Task<Results<CreatedAtRoute<UploadDumpMetadataResponse>, BadRequest>> UploadDumpMetadata(
        [FromBody] UploadDumpMetadataRequest request,
        [FromServices] ICommandHandler<UploadDumpMetadataCommand, Result<UploadDumpMetadataError, DumpId>> handler)
    {
        // TODO: faults
        var handleResult = await handler
            .Handle(new(new(new(request.Locator), request.FileName, request.TimeStamp)))
            .MapValue(id => TypedResults.CreatedAtRoute(
                new UploadDumpMetadataResponse(id.Value),
                DumpsModule.UploadDumpRouteName,
                new()
                {
                    ["dumpId"] = id.Value 
                }))
            .MapFault(_ => TypedResults.BadRequest());
                    
        return handleResult.TryGetValue(out var value, out var fault) 
            ? value 
            : fault;
    }
    
    private static async Task<Results<Ok<IReadOnlyList<UploadedDumpMetadata>>, BadRequest>> SearchDumpMetadata(
        [FromQuery] string locator,
        [FromServices] IQueryHandler<SearchMetadataQuery, Result<SearchDumpMetadataError, IReadOnlyList<UploadedDumpMetadata>>> handler)
    {
        // TODO: faults
        var handleResult = await handler
            .Handle(new(new(locator)))
            .MapValue(metadatas => TypedResults.Ok(metadatas))
            .MapFault(_ => TypedResults.BadRequest());
                    
        return handleResult.TryGetValue(out var value, out var fault) 
            ? value 
            : fault;
    }
}