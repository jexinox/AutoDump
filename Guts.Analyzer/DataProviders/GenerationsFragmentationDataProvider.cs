using Microsoft.Diagnostics.Runtime;

namespace Guts.Analyzer.DataProviders;

public class GenerationsFragmentationDataProvider
{
    private readonly IClrRuntimeProvider _runtimeProvider;
    private readonly ObjectsTreeProvider _objectsTreeProvider;

    public GenerationsFragmentationDataProvider(
        IClrRuntimeProvider runtimeProvider,
        ObjectsTreeProvider objectsTreeProvider)
    {
        _objectsTreeProvider = objectsTreeProvider;
        _runtimeProvider = runtimeProvider;
    }

    public IReadOnlyDictionary<Generation, (double Total, double Used)> Get()
    {
        var heap = _runtimeProvider.GetCurrentDumpRuntime().Heap;
        var objectsTree = _objectsTreeProvider.Get();
        var result = new Dictionary<Generation, (double Total, double Used)>();
        foreach (var segment in heap.Segments)
        {
            var segmentGenSizes = new Dictionary<Generation, (double Total, double Used)>();
            foreach (var obj in segment.EnumerateObjects().Where(obj => objectsTree.ObjectNodes.ContainsKey(obj)))
            {
                var generation = segment.GetGeneration(obj);
                var genSizeInSegment = GetGenerationSize(segment, generation);

                segmentGenSizes[generation] = segmentGenSizes.TryGetValue(generation, out var sizes)
                    ? (sizes.Total, sizes.Used + obj.Size)
                    : (genSizeInSegment, obj.Size);
            }

            foreach (var (generation, genSize) in segmentGenSizes)
            {
                result[generation] = result.TryGetValue(generation, out var sizes)
                    ? (sizes.Total + genSize.Total, sizes.Used + genSize.Used)
                    : (genSize.Total, genSize.Used);
            }
        }
        
        return result;
    }

    // refactor to more readable logic
    private static ulong GetGenerationSize(ClrSegment segment, Generation generation)
    {
        if (segment.Kind <= GCSegmentKind.Frozen) // hack from segment.GetGeneration
        {
            return segment.CommittedMemory.Length;
        }

        if (segment.Kind != GCSegmentKind.Ephemeral)
        {
            throw new NotSupportedException($"Segment kind {segment.Kind} is not supported");
        }

        return generation switch
        {
            Generation.Generation0 => segment.Generation0.Length,
            Generation.Generation1 => segment.Generation1.Length,
            Generation.Generation2 => segment.Generation2.Length,
            _ => throw new ArgumentOutOfRangeException(
                nameof(generation),
                generation,
                "Segment doesn't contain provided generation")
        };
    }
}