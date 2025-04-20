using Guts.Server.Dumps.FeatureModels;

namespace Guts.Server.Dumps.UploadDump;

public record UploadDumpCommand(DumpId DumpId, Dump Dump);