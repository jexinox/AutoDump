using Microsoft.Diagnostics.Runtime;

namespace Guts.Analyzer;

public class ObjectNode
{
    public ObjectNode(ClrObject obj, IReadOnlyList<ObjectNode> children, ulong retainedSize)
    {
        Object = obj;
        Children = children;
        RetainedSize = retainedSize;
    }
    
    public ClrObject Object { get; }
    
    public ulong RetainedSize { get; }
    
    public IReadOnlyList<ObjectNode> Children { get; }
}