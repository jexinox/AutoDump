namespace AutoDump.DumpAnalyzer;

public class DumpDominatorsTree
{
    internal DumpDominatorsTree(IReadOnlyList<DumpDominator> roots)
    {
        Roots = roots;
    }

    public IReadOnlyList<DumpDominator> Roots { get; }
    
    public static DumpDominatorsTree Create(DumpObjectsGraph graph) => new DumpDominatorsTreeBuilder().Build(graph);
}