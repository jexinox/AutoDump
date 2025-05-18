namespace AutoDump.Server.DumpsMetadata.FeatureModels;

public record UploadedDumpMetadata(DumpId DumpId, Locator Locator, string FileName, DateTimeOffset TimeStamp);