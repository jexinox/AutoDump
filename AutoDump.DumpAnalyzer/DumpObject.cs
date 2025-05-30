﻿using Microsoft.Diagnostics.Runtime;

namespace AutoDump.DumpAnalyzer;

public class DumpObject
{
    private DumpObject(
        ulong size,
        ClrType type,
        bool isBoxed,
        IReadOnlyList<DumpObject> references,
        GenerationType generation,
        int numberByTimeInDfs)
    {
        Size = size;
        Type = type;
        References = references;
        GenerationType = generation;
        NumberByTimeInDfs = numberByTimeInDfs;
    }

    internal static DumpObject Create(ClrHeap heap, ClrObject obj, IReadOnlyList<DumpObject> references, int numberByTimeInDfs)
    {
        var generation = heap.GetSegmentByAddress(obj)!.GetGeneration(obj) switch
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
        
        return new(obj.Size, obj.Type!, obj.IsBoxedValue, references, generation, numberByTimeInDfs);
    }

    public ulong Size { get; }
    
    public ClrType Type { get; }
    
    public bool IsBoxed { get; }
    
    public GenerationType GenerationType { get; }
    
    public IReadOnlyList<DumpObject> References { get; }
    
    internal int NumberByTimeInDfs { get; }
}