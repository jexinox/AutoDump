using Microsoft.Diagnostics.Runtime;

namespace Guts.Analyzer.DataProviders;

public class ObjectsDistributionByGenerationsDataProvider
{
    private readonly IClrRuntimeProvider _runtimeProvider;

    public ObjectsDistributionByGenerationsDataProvider(
        IClrRuntimeProvider runtimeProvider)
    {
        _runtimeProvider = runtimeProvider;
    }

    public IReadOnlyDictionary<Generation, IReadOnlyList<(MemoryRange GenerationRange, IReadOnlyList<ClrObject> ObjectsRanges)>> Get()
    {
        var heap = _runtimeProvider.GetCurrentDumpRuntime().Heap;
        var generationsRanges = new Dictionary<Generation, List<(MemoryRange, IReadOnlyList<ClrObject>)>>();

        foreach (var segmentGenerationRanges in heap.Segments.Select(GetGenerationsRangesBySegment))
        {
            foreach (var (generation, memoryRange) in segmentGenerationRanges)
            {
                var objects = GetObjectsInMemoryRange(memoryRange, heap);
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

    private static List<ClrObject> GetObjectsInMemoryRange(MemoryRange memoryRange, ClrHeap heap)
    {
        return heap
            .EnumerateObjects(memoryRange)
            .Where(obj => obj is { IsFree: false })
            .ToList();
    }
}