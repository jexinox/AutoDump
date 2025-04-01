using Microsoft.Diagnostics.Runtime;

namespace Guts.DumpAnalyzer;

public class DumpObjectsTree
{
    public static DumpObjectsTree Create(ClrHeap heap) => new DumpObjectsTreeBuilder(heap).Build();
    
    internal DumpObjectsTree(
        IReadOnlyList<DumpObject> roots,
        IReadOnlyDictionary<GenerationType, GenerationSizeInfo> generationsSizes,
        int count)
    {
        Roots = roots;
        GenerationsSizes = generationsSizes;
        Count = count;
    }
    
    public IReadOnlyList<DumpObject> Roots { get; }
    
    public IReadOnlyDictionary<GenerationType, GenerationSizeInfo> GenerationsSizes { get; }
    
    public int Count { get; }
}