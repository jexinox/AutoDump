using System.Diagnostics.CodeAnalysis;
using Microsoft.Diagnostics.Runtime;

namespace AutoDump.DumpAnalyzer;

internal class DumpObjectsGraphBuilder
{
    private readonly Dictionary<ulong, DumpObject> _builtObjects = new();
    private readonly Dictionary<GenerationType, MutableGenerationSizeInfo> _generationsSizeInfos = new()
    {
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

        foreach (var segment in _heap.Segments)
        {
            if (segment.Kind == GCSegmentKind.Ephemeral)
            {
                _generationsSizeInfos[GenerationType.Generation0].Total += segment.Generation0.Length;
                _generationsSizeInfos[GenerationType.Generation1].Total += segment.Generation1.Length;
                _generationsSizeInfos[GenerationType.Generation2].Total += segment.Generation2.Length;
            }
            else
            {
                _generationsSizeInfos[NonEphemeralSegmentKindToGenerationType(segment.Kind)].Total += segment.Length;
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

    private static GenerationType NonEphemeralSegmentKindToGenerationType(GCSegmentKind segmentKind)
    {
        return segmentKind switch
        {
            GCSegmentKind.Generation0 => GenerationType.Generation0,
            GCSegmentKind.Generation1 => GenerationType.Generation1,
            GCSegmentKind.Generation2 => GenerationType.Generation2,
            GCSegmentKind.Large => GenerationType.Large,
            GCSegmentKind.Pinned => GenerationType.Pinned,
            GCSegmentKind.Frozen => GenerationType.Frozen,
            _ => throw new ArgumentException($"Unknown segment kind: {segmentKind}"),
        };
    }

    private class MutableGenerationSizeInfo
    {
        public ulong Total;

        public ulong Used;
    }
}