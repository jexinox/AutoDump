namespace Guts.Server.Dumps.FeatureModels;

public record DumpMetadata(Locator Locator, string FileName, DateTimeOffset TimeStamp);