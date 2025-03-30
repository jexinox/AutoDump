using System.Diagnostics.CodeAnalysis;
using Microsoft.Diagnostics.Runtime;

namespace Guts.DumpAnalyzer;

public class DumpObjectsTree
{
    public static DumpObjectsTree Create(ClrHeap heap) => new DumpObjectsTreeBuilder().Build(heap);
    
    internal DumpObjectsTree(IReadOnlyList<DumpObject> roots, int count)
    {
        Roots = roots;
        Count = count;
    }
    
    public IReadOnlyList<DumpObject> Roots { get; }
    
    public int Count { get; }
}