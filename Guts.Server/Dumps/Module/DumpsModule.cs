using Guts.Models;
using Guts.Server.CQRS;
using Guts.Server.Dumps.Repositories;
using Guts.Server.Dumps.Upload;
using Guts.Server.Modules;
using Kontur.Results;
using Mapster;

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
        endpoints
            .MapPost(
                "/api/v1/dumps/{hostName}",
                async (
                    string hostName,
                    Stream dumpArchive,
                    ICommandHandler<UploadDumpCommand, Result<UploadDumpError>> handler) =>
                {
                    var command = new UploadDumpCommand(new(hostName), new(dumpArchive));
                    await handler.Handle(command);
                })
            .Accepts<Stream>("application/octet-stream");

        return endpoints;
    }
}