using Guts.Server.CQRS;
using Guts.Server.Dumps.Repositories;
using Guts.Server.Dumps.UploadDump;
using Guts.Server.Modules;
using Kontur.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Minio;

namespace Guts.Server.Dumps;

public class DumpsModule : IApiModule
{
    public const string UploadDumpRouteName = nameof(UploadDump);
    
    public IServiceCollection AddServices(IServiceCollection serviceCollection)
    {
        //TODO: configs
        serviceCollection
            .AddMinio("adminadmin", "adminadmin")
            .AddSingleton<IDumpsRepository, S3DumpsRepository>()
            .AddSingleton<ICommandHandler<UploadDumpCommand, Result<UploadDumpError>>, UploadDumpCommandHandler>();
        return serviceCollection;
    }
    
    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        endpoints
            .MapPut(ApiConstants.V1 + "/dumps/{dumpId:guid}", UploadDump)
            .WithName(UploadDumpRouteName);

        return endpoints;
    }

    private static async Task<Results<Ok, BadRequest>> UploadDump(
        [FromRoute] Guid dumpId,
        [FromServices] ICommandHandler<UploadDumpCommand, Result<UploadDumpError>> handler,
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

