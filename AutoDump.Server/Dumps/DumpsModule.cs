using AutoDump.Server.CQRS;
using AutoDump.Server.Dumps.Repositories;
using AutoDump.Server.Dumps.Upload;
using AutoDump.Server.Modules;
using Kontur.Results;
using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Minio;

namespace AutoDump.Server.Dumps;

public class DumpsModule : IApiModule
{
    public const string UploadDumpRouteName = nameof(UploadDump);
    
    public IServiceCollection AddServices(IServiceCollection serviceCollection)
    {
        //TODO: configs
        serviceCollection
            .AddMinio("adminadmin", "adminadmin")
            .AddSingleton<IDumpsRepository, S3DumpsRepository>()
            .AddSingleton<ICommandHandler<UploadDumpCommand, Result<UploadDumpError>>, UploadDumpCommandHandler>()
            .AddMassTransit(massTransit =>
            {
                massTransit.SetKebabCaseEndpointNameFormatter();
                massTransit.UsingRabbitMq((context, configurator) =>
                {
                    configurator.ConfigureEndpoints(context);
                });
            });
        
        serviceCollection
            .AddOptions<RabbitMqTransportOptions>()
            .BindConfiguration("RabbitMq");
        
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

