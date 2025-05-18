using AutoDump.Server.Dumps.FeatureModels;

namespace AutoDump.Server.Dumps.Upload;

public record UploadDumpCommand(DumpId DumpId, Dump Dump);