using Microsoft.Diagnostics.Runtime;

namespace Guts.Analyzer.Metrics;

public class ObjectsByGenerationsDistributionMetricProvider
{
    private readonly IClrRuntimeProvider _runtimeProvider;

    public ObjectsByGenerationsDistributionMetricProvider(IClrRuntimeProvider runtimeProvider)
    {
        _runtimeProvider = runtimeProvider;
    }

    public ILookup<Generation?, ClrObject> Get()
    {
        var runtime = _runtimeProvider.GetCurrentDumpRuntime();
        
        return runtime.Heap
            .EnumerateObjects()
            .Select(obj =>
            {
                var segment = runtime.Heap.GetSegmentByAddress(obj);
                return (Generation: segment?.GetGeneration(obj), Object: obj);
            })
            .ToLookup(pair => pair.Generation, pair => pair.Object);
    }
}