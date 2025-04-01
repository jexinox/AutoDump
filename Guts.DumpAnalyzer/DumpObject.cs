using Microsoft.Diagnostics.Runtime;

namespace Guts.DumpAnalyzer;

public class DumpObject
{
    private DumpObject(
        ulong size,
        ClrType type,
        IReadOnlyList<DumpObject> references,
        ClrSegment segment,
        GenerationType generation,
        int numberByTimeInDfs)
    {
        Size = size;
        Type = type;
        References = references;
        Segment = segment;
        GenerationType = generation;
        NumberByTimeInDfs = numberByTimeInDfs;
    }

    internal static DumpObject Create(ClrHeap heap, ClrObject obj, IReadOnlyList<DumpObject> references, int numberByTimeInDfs)
    {
        var segment = heap.GetSegmentByAddress(obj);

        var generation = segment!.GetGeneration(obj) switch
        {
            Generation.Unknown => GenerationType.Unknown,
            Generation.Generation0 => GenerationType.Generation0,
            Generation.Generation1 => GenerationType.Generation1,
            Generation.Generation2 => GenerationType.Generation2,
            Generation.Large => GenerationType.Large,
            Generation.Pinned => GenerationType.Pinned,
            Generation.Frozen => GenerationType.Frozen,
            _ => throw new ArgumentOutOfRangeException(nameof(obj))
        };
        
        return new(obj.Size, obj.Type!, references, segment, generation, numberByTimeInDfs);
    }

    public ulong Size { get; }
    
    public ClrType Type { get; }
    
    public ClrSegment Segment { get; }
    
    public GenerationType GenerationType { get; }
    
    public IReadOnlyList<DumpObject> References { get; }
    
    internal int NumberByTimeInDfs { get; }
}