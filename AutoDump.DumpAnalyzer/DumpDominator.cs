namespace AutoDump.DumpAnalyzer;

public class DumpDominator
{
    internal DumpDominator(DumpObject dumpObject, ulong retainedSize, IReadOnlyList<DumpDominator> references)
    {
        DumpObject = dumpObject;
        RetainedSize = retainedSize;
        References = references;
    }

    public DumpObject DumpObject { get; }
    
    public ulong RetainedSize { get; }
    
    public IReadOnlyList<DumpDominator> References { get; }
}