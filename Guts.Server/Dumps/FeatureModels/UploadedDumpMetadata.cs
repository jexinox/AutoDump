namespace Guts.Server.Dumps.FeatureModels;

public record UploadedDumpMetadata(DumpId DumpId, Locator Locator, string FileName, DateTimeOffset TimeStamp);