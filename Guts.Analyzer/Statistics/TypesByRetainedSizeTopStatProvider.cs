using Guts.Analyzer.DataProviders;

namespace Guts.Analyzer.Statistics;

public class TypesByRetainedSizeTopStatProvider
{
    private readonly ObjectsTreeFactory _factory;

    public TypesByRetainedSizeTopStatProvider(ObjectsTreeFactory factory)
    {
        _factory = factory;
    }

    public IEnumerable<(string Type, ulong Used)> Get()
    {
        var tree = _factory.GetTree();

        return tree
            .Roots
            .Select(node => (node.Object.Type?.Name, node.RetainedSize))
            .Where(pair => pair.Name is not null)
            .OfType<(string Name, ulong Retained)>()
            .GroupBy(pair => pair.Name)
            .Select(group => (
                Name: group.Key,
                Retained: group
                    .Select(dominator => dominator.Retained)
                    .Aggregate(0ul, (current, size) => current + size)))
            .OrderByDescending(group => group.Retained);
    }
}