using Guts.DumpAnalyzer.DataProviders;

namespace Guts.DumpAnalyzer.Statistics;

public class TypesBySizeTopStatProvider
{
    private readonly ObjectsTreeFactory _objectsTreeFactory;

    public TypesBySizeTopStatProvider(ObjectsTreeFactory objectsTreeFactory)
    {
        _objectsTreeFactory = objectsTreeFactory;
    }

    public IEnumerable<(string Type, ulong OccupiedSize)> Get()
    {
        var result = new Dictionary<string, ulong>();
        var objectsAndTypes = _objectsTreeFactory.GetTree()
            .Select(node => (node.Object.Size, node.Object.Type?.Name))
            .Where(pair => pair.Name is not null)
            .OfType<(ulong, string)>();
        foreach (var (size, typeName) in objectsAndTypes)
        {
            result[typeName] = result.TryGetValue(typeName, out var occupiedSize) 
                ? occupiedSize + size
                : size;
        }
        
        return result
            .Select(kvp => (kvp.Key, kvp.Value))
            .OrderByDescending(pair => pair.Value);
    }
}