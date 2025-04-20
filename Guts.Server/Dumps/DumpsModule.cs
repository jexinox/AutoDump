using Guts.Server.CQRS;
using Guts.Server.Dumps.FeatureModels;
using Guts.Server.Dumps.Repositories.Dumps;
using Guts.Server.Dumps.Repositories.Metadata;
using Guts.Server.Dumps.UploadDump;
using Guts.Server.Dumps.UploadMetadata;
using Guts.Server.Modules;
using Kontur.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Minio;
using MongoDB.Driver;

namespace Guts.Server.Dumps;

public class DumpsModule : IApiModule
{
    public IServiceCollection AddServices(IServiceCollection serviceCollection)
    {
        //TODO: configs
        serviceCollection
            .AddScoped<IMongoCollection<MongoDumpMetadata>>(
                sp => sp.GetRequiredService<IMongoDatabase>().GetCollection<MongoDumpMetadata>("DumpsMeta"))
            .AddScoped<IDumpsMetadataRepository, MongoDbMetadataRepository>()
            .AddScoped<
                ICommandHandler<UploadDumpMetadataCommand, Result<UploadDumpMetadataError, DumpId>>, UploadDumpMetadataHandler>();

        serviceCollection
            .AddMinio("adminadmin", "adminadmin")
            .AddSingleton<IDumpsRepository, S3DumpsRepository>()
            .AddSingleton<ICommandHandler<UploadDumpCommand, Result<UploadDumpError>>, UploadDumpCommandHandler>();
        return serviceCollection;
    }
    
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapPost(ApiConstants.V1 + "/dumps/metadatas", UploadDumpMetadata)
            .WithName(nameof(UploadDumpMetadata));

        endpoints
            .MapPut(ApiConstants.V1 + "/dumps/{dumpId:guid}", UploadDump)
            .WithName("UploadDump");

        return endpoints;
    }
    
    private record UploadDumpMetadataRequest(string Locator, string FileName, DateTimeOffset TimeStamp);

    private static async Task<Results<CreatedAtRoute, BadRequest>> UploadDumpMetadata(
        UploadDumpMetadataRequest request,
        ICommandHandler<UploadDumpMetadataCommand, Result<UploadDumpMetadataError, DumpId>> handler)
    {
        // TODO: faults
        var handleResult = await handler
            .Handle(new(new(new(request.Locator), request.FileName, request.TimeStamp)))
            .MapValue(id => TypedResults.CreatedAtRoute(nameof(UploadDump), new RouteValueDictionary
            {
                ["dumpId"] = id.Value 
            }))
            .MapFault(_ => TypedResults.BadRequest());
                    
        return handleResult.TryGetValue(out var value, out var fault) 
            ? value 
            : fault;
    }

    private static async Task<Results<Ok, BadRequest>> UploadDump(
        Guid dumpId,
        ICommandHandler<UploadDumpCommand, Result<UploadDumpError>> handler,
        HttpContext httpContext)
    {
        // TODO: faults and safety
        var handleResult = await handler
            .Handle(new(new(dumpId), new(httpContext.Request.Body)))
            .MapFault(_ => TypedResults.BadRequest());
                    
        return handleResult.TryGetFault(out var fault) 
            ? fault
            : TypedResults.Ok();
    }
}

