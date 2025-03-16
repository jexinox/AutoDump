using Microsoft.Diagnostics.Runtime;

namespace Guts.Analyzer.DataProviders;

public class ObjectsTreeProvider
{
    private readonly IClrRuntimeProvider _runtimeProvider;

    public ObjectsTreeProvider(IClrRuntimeProvider runtimeProvider)
    {
        _runtimeProvider = runtimeProvider;
    }

    public ObjectsTree Get()
    {
        var heap = _runtimeProvider.GetCurrentDumpRuntime().Heap;
        var visited = new HashSet<ulong>();
        var memo = new Dictionary<ulong, ObjectNode>();
        var roots = heap
            .EnumerateRoots()
            .Select(root => GetObjectNode(root.Object, visited, memo))
            .OfType<ObjectNode>()
            .ToList();

        return new(roots, memo);
    }

    private static ObjectNode? GetObjectNode(ClrObject obj, HashSet<ulong> visited, Dictionary<ulong, ObjectNode> memo)
    {
        if (obj.IsFree)
        {
            return null;
        }
        
        if (!visited.Add(obj))
        {
            return memo[obj];
        }
        
        var children = new List<ObjectNode>();
        
        var currentNode = new ObjectNode(obj, children);
        memo[obj] = currentNode;

        children.AddRange(
            obj
                .EnumerateReferences()
                .Select(reference => GetObjectNode(reference, visited, memo))
                .OfType<ObjectNode>());

        return currentNode;
    }
}