using Microsoft.Diagnostics.Runtime;

namespace Guts.Analyzer.DataProviders;

public class ObjectsTreeFactory
{
    private readonly Lazy<ObjectsTree> _tree;

    public ObjectsTreeFactory(IClrRuntimeProvider runtimeProvider)
    {
        _tree = new(() => BuildTree(runtimeProvider.GetCurrentDumpRuntime().Heap));
    }
    
    public ObjectsTree GetTree() => _tree.Value;

    private static ObjectsTree BuildTree(ClrHeap heap)
    {
        var domTree = new DominatorTreeBuilder().Build(heap);
        
        var visited = new HashSet<ulong>();
        var root = GetObjectNode(1, heap, domTree, visited);

        return new(root.Children, visited.Contains);
    }

    private static ObjectNode GetObjectNode(ulong objAddress, ClrHeap heap, ILookup<ulong, ulong> domTree, HashSet<ulong> visited)
    {
        visited.Add(objAddress);
        var children = domTree[objAddress]
            .Select(reference => GetObjectNode(reference, heap, domTree, visited))
            .ToList();

        var currentObject = heap.GetObject(objAddress);
        var retainedSize = (currentObject == 1 ? 0 : currentObject.Size) +
                           children.Aggregate(0ul, (accumulator, currChild) => accumulator + currChild.RetainedSize);

        return new(currentObject, children, retainedSize);
    }
}