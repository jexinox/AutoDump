namespace Guts.Server.Dumps.Upload;

public record UploadDumpRequest(string HostName, string FileName, DateTimeOffset TimeStamp, IFormFile DumpArchive);