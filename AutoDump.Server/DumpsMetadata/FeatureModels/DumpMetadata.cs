namespace AutoDump.Server.DumpsMetadata.FeatureModels;

public record DumpMetadata(Locator Locator, string FileName, DateTimeOffset TimeStamp);