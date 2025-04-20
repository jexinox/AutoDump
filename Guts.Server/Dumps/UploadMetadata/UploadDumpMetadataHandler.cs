using Guts.Server.CQRS;
using Guts.Server.Dumps.FeatureModels;
using Guts.Server.Dumps.Repositories.Metadata;
using Kontur.Results;

namespace Guts.Server.Dumps.UploadMetadata;

public class UploadDumpMetadataHandler(IDumpsMetadataRepository dumpsMetadataRepository) :
    ICommandHandler<UploadDumpMetadataCommand, Result<UploadDumpMetadataError, DumpId>>
{
    public async Task<Result<UploadDumpMetadataError, DumpId>> Handle(UploadDumpMetadataCommand command)
    {
        return await dumpsMetadataRepository
            .LoadDumpMetadata(command.Metadata)
            .MapFault(dbError => new UploadDumpMetadataError());
    }
}

public record UploadDumpMetadataError;