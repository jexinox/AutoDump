using Microsoft.Diagnostics.Runtime;

namespace Guts.Analyzer.DataProviders;

public class ObjectsDistributionByGenerationsDataProvider
{
    private readonly IClrRuntimeProvider _runtimeProvider;
    private readonly ObjectsTreeProvider _objectsTreeProvider;

    public ObjectsDistributionByGenerationsDataProvider(
        IClrRuntimeProvider runtimeProvider,
        ObjectsTreeProvider objectsTreeProvider)
    {
        _runtimeProvider = runtimeProvider;
        _objectsTreeProvider = objectsTreeProvider;
    }

    public IReadOnlyDictionary<Generation, IReadOnlyList<(MemoryRange GenerationRange, IReadOnlyList<ClrObject> ObjectsRanges)>> Get()
    {
        var heap = _runtimeProvider.GetCurrentDumpRuntime().Heap;
        var generationsRanges = new Dictionary<Generation, List<(MemoryRange, IReadOnlyList<ClrObject>)>>();
        var objectsTree = _objectsTreeProvider.Get();

        foreach (var segmentGenerationRanges in heap.Segments.Select(GetGenerationsRangesBySegment))
        {
            foreach (var (generation, memoryRange) in segmentGenerationRanges)
            {
                var objects = GetObjectsInMemoryRange(memoryRange, heap, objectsTree.ObjectNodes.ContainsKey);
                if (generationsRanges.TryGetValue(generation, out var generationRanges))
                {
                    generationRanges.Add(new(memoryRange, objects));
                }
                else
                {
                    generationsRanges[generation] = [new(memoryRange, objects)];
                }
            }
        }

        return generationsRanges
            .ToDictionary(
                generationRanges => generationRanges.Key, 
                IReadOnlyList<(MemoryRange, IReadOnlyList<ClrObject>)> (generationRanges) => generationRanges.Value);
    }

    private static Dictionary<Generation, MemoryRange> GetGenerationsRangesBySegment(ClrSegment segment)
    {
        if (segment.Kind <= GCSegmentKind.Frozen)
        {
            return new() { [(Generation)segment.Kind] = segment.CommittedMemory, };
        }

        if (segment.Kind != GCSegmentKind.Ephemeral)
        {
            throw new NotSupportedException($"Segment kind {segment.Kind} is not supported");
        }

        return new()
        {
            [Generation.Generation0] = segment.Generation0,
            [Generation.Generation1] = segment.Generation1,
            [Generation.Generation2] = segment.Generation2
        };
    }

    private static List<ClrObject> GetObjectsInMemoryRange(MemoryRange memoryRange, ClrHeap heap, Func<ulong, bool> isReachable)
    {
        return heap
            .EnumerateObjects(memoryRange)
            .Where(obj => isReachable(obj))
            .ToList();
    }
}