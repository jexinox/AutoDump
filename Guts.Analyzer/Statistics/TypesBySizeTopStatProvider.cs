using Guts.Analyzer.DataProviders;
using Microsoft.Diagnostics.Runtime;

namespace Guts.Analyzer.Statistics;

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
        var objectsAndTypes = _objectsTreeFactory.GetTree().ObjectNodes.Values
            .Select(node => (node.Object, node.Object.Type?.Name))
            .Where(pair => pair.Name is not null)
            .OfType<(ClrObject, string)>();
        foreach (var (obj, typeName) in objectsAndTypes)
        {
            result[typeName] = result.TryGetValue(typeName, out var occupiedSize) 
                ? occupiedSize + obj.Size 
                : obj.Size;
        }
        
        return result
            .Select(kvp => (kvp.Key, kvp.Value))
            .OrderByDescending(pair => pair.Value);
    }
}