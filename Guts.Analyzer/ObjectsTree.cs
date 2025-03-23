using System.Collections;

namespace Guts.Analyzer;

public class ObjectsTree : IEnumerable<ObjectNode>
{
    private readonly IReadOnlyDictionary<ulong, ObjectNode> _objectNodes;
    
    public ObjectsTree(IReadOnlyList<ObjectNode> roots, IReadOnlyDictionary<ulong, ObjectNode> objectNodes)
    {
        Roots = roots;
        _objectNodes = objectNodes;
    }

    public IReadOnlyList<ObjectNode> Roots { get; }

    public bool ContainsObject(ulong objectAddress) => _objectNodes.ContainsKey(objectAddress);
    
    public IEnumerator<ObjectNode> GetEnumerator() => _objectNodes.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}