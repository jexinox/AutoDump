using System.Diagnostics.CodeAnalysis;
using Microsoft.Diagnostics.Runtime;

namespace Guts.DumpAnalyzer;

internal class DumpObjectsGraphBuilder
{
    private readonly Dictionary<ulong, DumpObject> _builtObjects = new();
    private readonly HashSet<ClrSegment> _visitedSegments = [];
    private readonly Dictionary<GenerationType, MutableGenerationSizeInfo> _generationsSizeInfos = new()
    {
        [GenerationType.Unknown] = new(),
        [GenerationType.Generation0] = new(),
        [GenerationType.Generation1] = new(),
        [GenerationType.Generation2] = new(),
        [GenerationType.Large] = new(),
        [GenerationType.Pinned] = new(),
        [GenerationType.Frozen] = new(),
    };
    
    private readonly ClrHeap _heap;

    public DumpObjectsGraphBuilder(ClrHeap heap)
    {
        _heap = heap;
    }
    
    private int _globalNodeNumber = DumpDominatorsTreeBuilder.FakeRoot;

    public DumpObjectsGraph Build()
    {
        var dumpObjectRoots = new List<DumpObject>();
        foreach (var root in _heap.EnumerateRoots())
        {
            if (TryGetDumpObject(root.Object, out var rootDumpObject))
            {
                dumpObjectRoots.Add(rootDumpObject);
            }
        }
            
        return new(
            dumpObjectRoots,
            _generationsSizeInfos
                .ToDictionary(kvp => kvp.Key, kvp => new GenerationSizeInfo(kvp.Value.Total, kvp.Value.Used)),
            _builtObjects.Count);
    }
    
    private bool TryGetDumpObject(
        ClrObject currentObject,
        [NotNullWhen(true)] out DumpObject? dumpObject)
    {
        if (currentObject.IsNull)
        {
            dumpObject = null;
            return false;
        }
            
        if (_builtObjects.TryGetValue(currentObject, out var builtDumpObject))
        {
            dumpObject = builtDumpObject;
            return true;
        }
    
        var currentObjectReferences = new List<DumpObject>();
        var currentNodeNumber = ++_globalNodeNumber;
        var currentDumpObj = DumpObject.Create(_heap, currentObject, currentObjectReferences, currentNodeNumber);
        AdjustGenerationSizeInfo(currentDumpObj);
        _builtObjects[currentObject] = currentDumpObj;
        
        foreach (var reference in currentObject.EnumerateReferences())
        {
            if (!TryGetDumpObject(reference, out var referencedDumpObject))
            {
                continue;
            }
            
            currentObjectReferences.Add(referencedDumpObject);
        }
    
        dumpObject = currentDumpObj;
        return true;
    }

    private void AdjustGenerationSizeInfo(DumpObject dumpObject)
    {
        var genSizeInfo = _generationsSizeInfos[dumpObject.GenerationType];
        
        genSizeInfo.Used += dumpObject.Size;
    }

    private class MutableGenerationSizeInfo
    {
        public ulong Total;

        public ulong Used;
    }
}