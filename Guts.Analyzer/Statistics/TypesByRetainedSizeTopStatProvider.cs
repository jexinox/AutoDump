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
        var objectsNumbers = new Dictionary<ulong, ulong>();
        var visited = new HashSet<ulong>();
        foreach (var root in tree.Roots)
        {
            NumerateObjects(root, visited, objectsNumbers, 1);
        }

        return new (string Type, ulong Used)[1];
    }

    private static void NumerateObjects(ObjectNode node, HashSet<ulong> visited, Dictionary<ulong, ulong> objectsNumbers, ulong number)
    {
        if (!visited.Add(node.Object))
        {
            return;
        }
        
        objectsNumbers[node.Object] = number;
        foreach (var child in node.Children)
        {
            NumerateObjects(child, visited, objectsNumbers, ++number);
        }
    }
}