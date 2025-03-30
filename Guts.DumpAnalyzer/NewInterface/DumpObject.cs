using Microsoft.Diagnostics.Runtime;

namespace Guts.DumpAnalyzer;

public class DumpObject
{
    internal DumpObject(ClrObject obj, IReadOnlyList<DumpObject> references, int numberByTimeInDfs)
    {
        Object = obj;
        References = references;
        NumberByTimeInDfs = numberByTimeInDfs;
    }

    public ClrObject Object { get; }
    
    public IReadOnlyList<DumpObject> References { get; }
    
    internal int NumberByTimeInDfs { get; }
}