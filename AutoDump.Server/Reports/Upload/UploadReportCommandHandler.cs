using AutoDump.Server.CQRS;
using AutoDump.Server.Reports.Repositories;
using Kontur.Results;

namespace AutoDump.Server.Reports.Upload;

public class UploadReportCommandHandler(IReportsRepository repository) : ICommandHandler<UploadReportCommand, Result<UploadReportError>>
{
    public async Task<Result<UploadReportError>> Handle(UploadReportCommand command)
    {
        return await repository
            .Upload(command.Report)
            .MapFault(dbError => new UploadReportError());
    }
}

public record UploadReportError;