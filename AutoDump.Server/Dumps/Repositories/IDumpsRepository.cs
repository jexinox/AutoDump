using AutoDump.Server.Dumps.FeatureModels;
using Kontur.Results;

namespace AutoDump.Server.Dumps.Repositories;

public interface IDumpsRepository
{
    Task<Result<RepositoryUploadDumpError>> LoadDump(DumpId id, Dump dump);
}

public record RepositoryUploadDumpError;
