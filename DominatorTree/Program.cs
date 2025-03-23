var succ = new List<List<int>>
{
    new(),
    new() { 2, 3 },
    new() { 4, 5, 6 },
    new() { 7 },
    new(),
    new() { 9 },
    new() { 3 },
    new() { 6, 8 },
    new() { 5 },
    new() 
};

// List<List<int>> succ =
// [
//     [],
//     [2, 3],
//     [4, 5],
//     [6],
//     [],
//     [3],
//     [5]
// ];
var tree = DominatorTree.Create(succ);
tree.Iterate();

public class DominatorTree
{
    private readonly Vertex[] _vertices;
    
    private readonly int[] _originalNumByTimeIn;

    private readonly List<List<int>> _originalLinksList;
    private readonly int _nodesCount;

    private DominatorTree(Vertex[] vertices, int[] originalNumByTimeIn, List<List<int>> originalLinksList, int nodesCount)
    {
        _vertices = vertices;
        _originalNumByTimeIn = originalNumByTimeIn;

        _originalLinksList = originalLinksList;
        _nodesCount = nodesCount;
    }

    public static DominatorTree Create(List<List<int>> originalLinksList)
    {
        var n = originalLinksList.Count;
        var vertices = new Vertex[n];
        for (var i = 0; i < n; i++)
        {
            vertices[i] = new();
        }
        
        return new(vertices, new int[n], originalLinksList, n);
    }

    public void Iterate()
    {
        var reachableVertices = new HashSet<int>();
        NumerateVerticesWithTimeInByDfs(1, new(), reachableVertices);

        var linkEval = UnbalancedLinkEvalTree.Create(_nodesCount, reachableVertices, vertex => _vertices[vertex].Semidominator);
        for (var i = _nodesCount - 1; i > 1; i--)
        {
            var w = _originalNumByTimeIn[i];
            foreach (var u in _vertices[w].Predecessors
                         .Select(v => linkEval.Eval(v))
                         .Where(u => _vertices[u].Semidominator < _vertices[w].Semidominator))
            {
                _vertices[w].Semidominator = _vertices[u].Semidominator;
            }
            _vertices[_originalNumByTimeIn[_vertices[w].Semidominator]].Bucket.Add(w);
            linkEval.Link(_vertices[w].Parent, w);
            foreach (var v in _vertices[_vertices[w].Parent].Bucket)
            {
                var u = linkEval.Eval(v);
                _vertices[v].Dominator = 
                    _vertices[u].Semidominator < _vertices[v].Semidominator 
                        ? u 
                        : _vertices[w].Parent;
            }
        }

        for (var i = 2; i < _nodesCount; i++)
        {
            var w = _originalNumByTimeIn[i];
            if (_vertices[w].Dominator != _originalNumByTimeIn[_vertices[w].Semidominator])
            {
                _vertices[w].Dominator = _vertices[_vertices[w].Dominator].Dominator;
            }
        }

        _vertices[1].Dominator = 0;
        
        foreach (var dominator in _vertices.Select(vertex => vertex.Dominator).Skip(1))
        {
            Console.WriteLine(dominator);
        }
    }
    
    private void NumerateVerticesWithTimeInByDfs(int v, VerticesNumerator numerator, HashSet<int> reachable)
    {
        _vertices[v].Semidominator = numerator.AssignNewNumber();
        reachable.Add(v);
        _originalNumByTimeIn[numerator.Value] = v;
        foreach (var w in _originalLinksList[v])
        {
            if (_vertices[w].Semidominator == 0)
            {
                _vertices[w].Parent = v;
                NumerateVerticesWithTimeInByDfs(w, numerator, reachable);
            }
            _vertices[w].Predecessors.Add(v);
        }
    }
    
    private class Vertex
    {
        public int Dominator;

        public int Semidominator;

        public int Parent;
        
        public readonly List<int> Predecessors = new();
        
        public readonly List<int> Bucket = new();
    }
    
    private class VerticesNumerator
    {
        public int Value { get; private set; }

        public int AssignNewNumber() => ++Value;
    }
}

public class UnbalancedLinkEvalTree
{
    private readonly LinkEvalVertex[] _tree;
    private readonly Func<int, int> _evalFunction;
    
    private UnbalancedLinkEvalTree(LinkEvalVertex[] tree, Func<int, int> evalFunction)
    {
        _tree = tree;
        _evalFunction = evalFunction;
    }
    
    public static UnbalancedLinkEvalTree Create(int verticesCount, ISet<int> reachableVertices, Func<int, int> evalFunction)
    {
        var tree = new LinkEvalVertex[verticesCount];

        for (var i = 1; i < verticesCount; i++)
        {
            if (reachableVertices.Contains(i))
            {
                tree[i] = new()
                {
                    Vertex = i,
                    Label = i,
                    Ancestor = new() { Vertex = 0 }
                };
            }
        }
        
        return new(tree, evalFunction);
    }
    
    public int Eval(int v)
    {
        var vertex = _tree[v];
        if (vertex.Ancestor.Vertex == 0)
        {
            return v;
        }

        Compress(vertex);
        return vertex.Label;
    }

    public void Link(int v, int w)
    {
        _tree[w].Ancestor = _tree[v];
    }
    
    private void Compress(LinkEvalVertex v)
    {
        if (v.Ancestor.Ancestor.Vertex == 0)
        {
            return;
        }
        
        Compress(v.Ancestor);
        if (_evalFunction(v.Ancestor.Label) < _evalFunction(v.Label))
        {
            v.Label = v.Ancestor.Label;
        }

        v.Ancestor = v.Ancestor.Ancestor;
    }
    
    private class LinkEvalVertex
    {
        public int Vertex;

        public int Label;
        
        public LinkEvalVertex Ancestor = null!;
    }
}