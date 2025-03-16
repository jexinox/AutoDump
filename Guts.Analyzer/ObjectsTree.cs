namespace Guts.Analyzer;

public class ObjectsTree
{
    public ObjectsTree(IReadOnlyList<ObjectNode> roots, IReadOnlyDictionary<ulong, ObjectNode> objectNodes)
    {
        Roots = roots;
        ObjectNodes = objectNodes;
    }

    public IReadOnlyList<ObjectNode> Roots { get; }
    
    public IReadOnlyDictionary<ulong, ObjectNode> ObjectNodes { get; }
}