using Microsoft.AspNetCore.Mvc;

namespace Guts.Server.Dumps.Module;

public record UploadDumpRequest([FromRoute] string HostName, [FromBody] Stream DumpArchive);