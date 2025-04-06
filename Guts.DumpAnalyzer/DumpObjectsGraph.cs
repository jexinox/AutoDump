using Microsoft.Diagnostics.Runtime;

namespace Guts.DumpAnalyzer;

public class DumpObjectsGraph
{
    public static DumpObjectsGraph Create(ClrHeap heap) => new DumpObjectsGraphBuilder(heap).Build();
    
    internal DumpObjectsGraph(
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

    public IEnumerable<DumpObject> EnumerateByDfs()
    {
        var visited = new HashSet<DumpObject>();
        var stack = new Stack<DumpObject>(Roots.Reverse());

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (!visited.Add(current))
            {
                continue;
            }
            
            yield return current;

            foreach (var child in current.References.Reverse())
            {
                stack.Push(child);
            }
        }
    }
}