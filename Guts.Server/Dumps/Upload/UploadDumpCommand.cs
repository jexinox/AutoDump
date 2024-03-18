using Guts.Models;

namespace Guts.Server.Dumps.Upload;

public record UploadDumpCommand(DumpMetadata Metadata, DumpArchive DumpArchive);