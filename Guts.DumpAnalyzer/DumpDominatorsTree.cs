namespace Guts.DumpAnalyzer;

public class DumpDominatorsTree
{
    internal DumpDominatorsTree(IReadOnlyList<DumpDominator> roots)
    {
        Roots = roots;
    }

    public IReadOnlyList<DumpDominator> Roots { get; }
    
    public static DumpDominatorsTree Create(DumpObjectsTree tree) => new DumpDominatorsTreeBuilder().Build(tree);
}