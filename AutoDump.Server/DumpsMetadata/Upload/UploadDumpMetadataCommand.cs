using AutoDump.Server.DumpsMetadata.FeatureModels;

namespace AutoDump.Server.DumpsMetadata.Upload;

public record UploadDumpMetadataCommand(DumpMetadata Metadata);