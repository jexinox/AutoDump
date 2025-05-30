using AutoDump.EventsModels;
using AutoDump.Server.CQRS;
using AutoDump.Server.Dumps.Repositories;
using Kontur.Results;
using MassTransit;

namespace AutoDump.Server.Dumps.Upload;

public class UploadDumpCommandHandler(IDumpsRepository repository, IBus bus) : ICommandHandler<UploadDumpCommand, Result<UploadDumpError>>
{
    public async Task<Result<UploadDumpError>> Handle(UploadDumpCommand command)
    {
        var (id, dump) = command;
        var uploadResult = await repository
            .LoadDump(id, dump)
            .MapFault(dbError => new UploadDumpError());

        await bus.Publish(new UploadedDumpEvent(id.Value));
        
        return uploadResult;
    }
}

public record UploadDumpError;