using Guts.Server.CQRS;
using Guts.Server.Dumps.Repositories;
using Kontur.Results;

namespace Guts.Server.Dumps.UploadDump;

public class UploadDumpCommandHandler(IDumpsRepository repository) : ICommandHandler<UploadDumpCommand, Result<UploadDumpError>>
{
    public async Task<Result<UploadDumpError>> Handle(UploadDumpCommand command)
    {
        var (id, dump) = command;
        return await repository
            .LoadDump(id, dump)
            .MapFault(dbError => new UploadDumpError());
    }
}

public record UploadDumpError;