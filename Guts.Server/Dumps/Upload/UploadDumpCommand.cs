using Guts.Server.Dumps.FeatureModels;

namespace Guts.Server.Dumps.Upload;

public record UploadDumpCommand(DumpId DumpId, Dump Dump);