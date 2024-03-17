using Guts.Models;

namespace Guts.Server.Dumps.Upload;

public record UploadDumpCommand(HostName HostName, DumpArchive DumpArchive);