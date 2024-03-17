using Guts.Server.CQRS;
using Guts.Server.Dumps.Repositories;
using Kontur.Results;

namespace Guts.Server.Dumps.Upload;

public class UploadDumpHandler(IDumpsRepository dumpsRepository) : ICommandHandler<UploadDumpCommand, Result<UploadDumpError>>
{
    public async Task<Result<UploadDumpError>> Handle(UploadDumpCommand command)
    {
        var (hostName, dumpArchive) = command;
        
        return await dumpsRepository
            .LoadDump(hostName, dumpArchive)
            .MapFault(dbError => new UploadDumpError());
    }
}