using Guts.Analyzer.DataProviders;
using Microsoft.Diagnostics.Runtime;

namespace Guts.Analyzer.Statistics;

public class TypesTopStatProvider
{
    private readonly ObjectsTreeProvider _objectsTreeProvider;

    public TypesTopStatProvider(ObjectsTreeProvider objectsTreeProvider)
    {
        _objectsTreeProvider = objectsTreeProvider;
    }

    public IReadOnlyList<(string Type, ulong OccupiedSize)> Get()
    {
        var result = new Dictionary<string, ulong>();
        var objectsAndTypes = _objectsTreeProvider.Get().ObjectNodes.Values
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
            .OrderByDescending(pair => pair.Value)
            .ToList();
    }
}