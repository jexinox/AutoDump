using Guts.Server.CQRS;
using Guts.Server.Dumps.Repositories;
using Guts.Server.Dumps.Repositories.Mongo;
using Guts.Server.Dumps.Upload;
using Guts.Server.Modules;
using Kontur.Results;
using Microsoft.AspNetCore.Mvc;

namespace Guts.Server.Dumps;

public class DumpsModule : IApiModule
{
    public IServiceCollection AddServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<IDumpsRepository, MongoDbDumpsRepository>();
        
        serviceCollection.AddSingleton<ICommandHandler<UploadDumpCommand, Result<UploadDumpError>>, UploadDumpHandler>();

        return serviceCollection;
    }

    public IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        // TODO (jexinox): faults mapping
        endpoints
            .MapPost(
                "/api/v1/dumps",
                async (
                    [FromForm] UploadDumpRequest request,
                    ICommandHandler<UploadDumpCommand, Result<UploadDumpError>> handler) =>
                {
                    var command = new UploadDumpCommand(
                        new(new(request.HostName), new(request.FileName), new(request.TimeStamp)),
                        new(request.DumpArchive.OpenReadStream()));
                    return await handler.Handle(command).MapFault(_ => Results.Ok());
                })
            .Accepts<UploadDumpRequest>("multipart/form-data")
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return endpoints;
    }
}