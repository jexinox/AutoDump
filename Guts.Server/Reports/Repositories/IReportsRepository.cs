using Guts.Server.Reports.FeatureModels;
using Kontur.Results;

namespace Guts.Server.Reports.Repositories;

public interface IReportsRepository
{
    Task<Result<RepositoryUploadReportError>> Upload(Report report);
    
    Task<Result<RepositorySearchReportError, IReadOnlyCollection<Report>>> Search(DumpId dumpId);
}

public record RepositoryUploadReportError;

public record RepositorySearchReportError;