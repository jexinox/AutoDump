using Microsoft.Diagnostics.Runtime;

namespace Guts.Analyzer;

public class ObjectNode
{
    public ObjectNode(ClrObject obj, IReadOnlyList<ObjectNode> children)
    {
        Object = obj;
        Children = children;
    }

    public ClrObject Object { get; }
    
    public IReadOnlyList<ObjectNode> Children { get; }
}