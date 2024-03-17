using Guts.Server.CQRS;
using Guts.Server.Dumps.Repositories;
using Guts.Server.Dumps.Upload;
using Guts.Server.Modules;
using Kontur.Results;

namespace Guts.Server.Dumps.Module;

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
                "/api/v1/dumps/{hostName}",
                async (
                    string hostName,
                    Stream dumpArchive,
                    ICommandHandler<UploadDumpCommand, Result<UploadDumpError>> handler) =>
                {
                    var command = new UploadDumpCommand(new(hostName), new(dumpArchive));
                    return await handler.Handle(command).MapFault(_ => Results.Ok());
                })
            .Accepts<Stream>("application/octet-stream")
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return endpoints;
    }
}