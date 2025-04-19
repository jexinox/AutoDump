using Guts.Server.CQRS;
using Guts.Server.Dumps.FeatureModels;
using Guts.Server.Dumps.Upload;
using Guts.Server.Modules;
using Kontur.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Driver;

namespace Guts.Server.Dumps;

public class DumpsModule : IApiModule
{
    public IServiceCollection AddServices(IServiceCollection serviceCollection)
    {
        //TODO: configs
        serviceCollection.AddSingleton<IMongoClient>(_ => new MongoClient(
            new MongoUrl("mongodb://localhost:27017/?readPreference=primary&appname=guts.server&directConnection=true&ssl=false")));

        serviceCollection.AddSingleton<IMongoDatabase>(sp => sp.GetRequiredService<IMongoClient>().GetDatabase("Guts"));
        
        serviceCollection.AddScoped<
            ICommandHandler<UploadDumpMetadataCommand, Result<UploadDumpMetadataError, DumpId>>, UploadDumpMetadataHandler>();

        return serviceCollection;
    }
    
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapPost(ApiConstants.V1 + "/hosts/{hostName}/dumps", UploadDumpMetadata)
            .WithName(nameof(UploadDumpMetadata));

        endpoints
            .MapPut(ApiConstants.V1 + "/hosts/{hostName}/dumps/{dumpId:guid}", () => Results.Ok())
            .WithName("UploadDump");

        return endpoints;
    }
    
    private record UploadDumpMetadataRequest(string FileName, DateTimeOffset TimeStamp);

    private static async Task<Results<CreatedAtRoute, BadRequest>> UploadDumpMetadata(
        string hostName,
        UploadDumpMetadataRequest request,
        ICommandHandler<UploadDumpMetadataCommand, Result<UploadDumpMetadataError, DumpId>> handler)
    {
        // TODO: faults
        var handleResult = await handler
            .Handle(new(new(hostName, request.FileName, request.TimeStamp)))
            .MapValue(id => TypedResults.CreatedAtRoute("UploadDump", new RouteValueDictionary
            {
                ["hostName"] = hostName,
                ["dumpId"] = id.Value 
            }))
            .MapFault(_ => TypedResults.BadRequest());
                    
        return handleResult.TryGetValue(out var value, out var fault) 
            ? value 
            : fault;
    }
}

