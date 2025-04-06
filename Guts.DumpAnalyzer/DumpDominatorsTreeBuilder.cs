namespace Guts.DumpAnalyzer;

internal class DumpDominatorsTreeBuilder
{
    internal const int FakeRoot = 1; // DON'T CHANGE TO ZERO
    
    public DumpDominatorsTree Build(DumpObjectsGraph dumpObjectsGraph)
    {
        var n = dumpObjectsGraph.Count;
        var vertices = InitializeVertices(n, dumpObjectsGraph);
        CalculateDominators(vertices);
        return BuildDominatorsTree(vertices);
    }

    private static DumpDominatorsTree BuildDominatorsTree(Vertex[] vertices)
    {
        var n = vertices.Length;
        var roots = new List<MutableDumpDominator>(n / 2);
        var dominatorTree = new Dictionary<DumpObject, MutableDumpDominator>(n);
        foreach (var vertex in vertices.Skip(FakeRoot + 1))
        {
            var dominatorObj = new MutableDumpDominator(vertex.Original);
            dominatorTree[vertex.Original] = dominatorObj;

            if (vertex.Dominator == FakeRoot)
            {
                roots.Add(dominatorObj);
            }
            else
            {
                var dominator = dominatorTree[vertices[vertex.Dominator].Original]; 
                dominator.Children.Add(dominatorObj);
            }
        }

        return new(ConvertTree(roots));
    }

    private static List<DumpDominator> ConvertTree(List<MutableDumpDominator> roots)
    {
        return roots.Select(ConvertNode).ToList();

        static DumpDominator ConvertNode(MutableDumpDominator dominator)
        {
            var children = dominator.Children.Select(ConvertNode).ToList();
            return new(
                dominator.DumpObject, 
                dominator.DumpObject.Size + children.Aggregate(0ul, (current, child) => current + child.RetainedSize),
                children);
        }
    }

    private class MutableDumpDominator
    {
        public MutableDumpDominator(DumpObject dumpObject)
        {
            DumpObject = dumpObject;
        }
        
        public DumpObject DumpObject { get; }
        
        public readonly List<MutableDumpDominator> Children = [];
    }
    
    private static Vertex[] InitializeVertices(int n, DumpObjectsGraph dumpObjectsGraph)
    {
        var vertices = new Vertex[n + FakeRoot + 1];
        var visited = new HashSet<DumpObject>();
        vertices[FakeRoot] = new() { Semidominator = FakeRoot, Original = null! };
        foreach (var root in dumpObjectsGraph.Roots.Where(visited.Add))
        {
            InitChildrenWithDfs(root, visited, vertices);
            vertices[root.NumberByTimeInDfs].Parent = FakeRoot;
            vertices[root.NumberByTimeInDfs].Predecessors.Add(FakeRoot);
        }
        
        return vertices;

        static void InitChildrenWithDfs(DumpObject currentObj, HashSet<DumpObject> visited, Vertex[] vertices)
        {
            var nodeNumber = currentObj.NumberByTimeInDfs;
            vertices[currentObj.NumberByTimeInDfs] = new() { Semidominator = nodeNumber, Original = currentObj };
            foreach (var child in currentObj.References)
            {
                if (visited.Add(child))
                {
                    InitChildrenWithDfs(child, visited, vertices);
                    vertices[child.NumberByTimeInDfs].Parent = nodeNumber;
                }
            
                vertices[child.NumberByTimeInDfs].Predecessors.Add(nodeNumber);
            }
        }
    }

    private static void CalculateDominators(Vertex[] vertices)
    {
        var n = vertices.Length;
        var linkEvalTree = new UnbalancedLinkEvalTree(vertices, address => vertices[address].Semidominator);
        for (var i = n - 1; i > FakeRoot; i--)
        {
            foreach (var u in vertices[i].Predecessors
                         .Select(linkEvalTree.Eval)
                         .Where(u => vertices[u].Semidominator < vertices[i].Semidominator))
            {
                vertices[i].Semidominator = vertices[u].Semidominator;
            }
            
            vertices[vertices[i].Semidominator].Bucket.Add(i);
            linkEvalTree.Link(vertices[i].Parent, i);
            foreach (var v in vertices[vertices[i].Parent].Bucket)
            {
                var u = linkEvalTree.Eval(v);
                vertices[v].Dominator = 
                    vertices[u].Semidominator < vertices[v].Semidominator 
                        ? u 
                        : vertices[i].Parent;
            }
        }
        
        for (var i = FakeRoot + 1; i < n; i++)
        {
            if (vertices[i].Dominator != vertices[i].Semidominator)
            {
                vertices[i].Dominator = vertices[vertices[i].Dominator].Dominator;
            }
        }
    }
    
    private class UnbalancedLinkEvalTree
    {
        private readonly LinkEvalVertex[] _tree;
        private readonly Func<int, int> _evalFunction;
        
        public UnbalancedLinkEvalTree(Vertex[] vertices, Func<int, int> evalFunction)
        {
            _tree = new LinkEvalVertex[vertices.Length];
            for (var i = FakeRoot; i < vertices.Length; i++)
            {
                _tree[i] = new()
                {
                    Label = vertices[i].Semidominator,
                    Ancestor = 0
                };
            }
            
            _evalFunction = evalFunction;
        }
        
        public int Eval(int v)
        {
            ref var vertex = ref _tree[v];
            if (vertex.Ancestor == 0)
            {
                return v;
            }
    
            Compress(ref vertex);
            return vertex.Label;
        }
    
        public void Link(int v, int w)
        {
            _tree[w].Ancestor = v;
        }
        
        private void Compress(ref LinkEvalVertex v)
        {
            ref var ancestor = ref _tree[v.Ancestor];
            if (ancestor.Ancestor == 0)
            {
                return;
            }
            
            Compress(ref ancestor);
            if (_evalFunction(ancestor.Label) < _evalFunction(v.Label))
            {
                v.Label = ancestor.Label;
            }
    
            v.Ancestor = ancestor.Ancestor;
        }
        
        private struct LinkEvalVertex
        {
            public int Label;

            public int Ancestor;
        }
    }
    
    private class Vertex
    {
        public required DumpObject Original;
        
        public int Dominator;

        public int Semidominator;

        public int Parent;
        
        public readonly List<int> Predecessors = [];
        
        public readonly List<int> Bucket = [];
    }
}