using System.Collections;

namespace Guts.Analyzer;

public class ObjectsTree : IEnumerable<ObjectNode>
{
    private readonly Func<ulong, bool> _isContained;
    
    public ObjectsTree(IReadOnlyList<ObjectNode> roots, Func<ulong, bool> isContained)
    {
        _isContained = isContained;
        Roots = roots;
    }

    public IReadOnlyList<ObjectNode> Roots { get; }

    public bool ContainsObject(ulong objectAddress) => _isContained(objectAddress);
    
    public IEnumerator<ObjectNode> GetEnumerator()
    {
        var queue = new Queue<ObjectNode>(Roots);
        
        while (queue.Count != 0)
        {
            var current = queue.Dequeue();
            yield return current;
            foreach (var child in current.Children)
            {
                queue.Enqueue(child);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}