using AutoDump.Server.Reports.FeatureModels;
using Kontur.Results;

namespace AutoDump.Server.Reports.Repositories;

public interface IReportsRepository
{
    Task<Result<RepositoryUploadReportError>> Upload(Report report);
    
    Task<Result<RepositorySearchReportError, IReadOnlyList<Report>>> Search(DumpId dumpId);
}

public record RepositoryUploadReportError;

public record RepositorySearchReportError;