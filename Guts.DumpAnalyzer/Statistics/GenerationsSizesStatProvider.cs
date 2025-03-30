using Guts.DumpAnalyzer.DataProviders;
using Microsoft.Diagnostics.Runtime;

namespace Guts.DumpAnalyzer.Statistics;

public class GenerationsSizesStatProvider
{
    private readonly ObjectsDistributionByGenerationsDataProvider _objectsDistributionByGenerationsDataProvider;

    public GenerationsSizesStatProvider(
        ObjectsDistributionByGenerationsDataProvider objectsDistributionByGenerationsDataProvider)
    {
        _objectsDistributionByGenerationsDataProvider = objectsDistributionByGenerationsDataProvider;
    }

    public IReadOnlyDictionary<Generation, (double Total, double Used)> Get()
    {
        var objectsDistributionByGenerationsData = _objectsDistributionByGenerationsDataProvider.Get();
        var result = new Dictionary<Generation, (double Total, double Used)>();
        foreach (var (generation, generationRanges) in objectsDistributionByGenerationsData)
        {
            var total = 0ul;
            var used = 0ul;
            foreach (var (generationRange, objectsRanges) in generationRanges)
            {
                total += generationRange.Length;
                used = objectsRanges.Aggregate(used, (current, obj) => current + obj.Size);
            }
            result.Add(generation, (total, used));
        }
        
        return result;
    }
}