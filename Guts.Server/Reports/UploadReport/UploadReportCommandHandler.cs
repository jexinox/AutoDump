using Guts.Server.CQRS;
using Guts.Server.Reports.Repositories;
using Kontur.Results;

namespace Guts.Server.Reports.UploadReport;

public class UploadReportCommandHandler(IReportsRepository repository) : ICommandHandler<UploadReportCommand, Result<UploadReportCommandError>>
{
    public async Task<Result<UploadReportCommandError>> Handle(UploadReportCommand command)
    {
        return await repository
            .Upload(command.Report)
            .MapFault(dbError => new UploadReportCommandError());
    }
}

public record UploadReportCommandError;