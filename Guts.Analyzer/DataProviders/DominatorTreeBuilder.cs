using Microsoft.Diagnostics.Runtime;

namespace Guts.Analyzer.DataProviders;

public class DominatorTreeBuilder
{
    private readonly Dictionary<ulong, Vertex> _vertices = new();
    private readonly Dictionary<int, ulong> _originalByTimeIn = new();
        
    public ILookup<ulong, ulong> Build(ClrHeap heap)
    {
        var roots = heap.EnumerateRoots().ToList();
        var numerator = new VerticesNumerator();
        var rootNum = numerator.AssignNewNumber();
        _vertices[1] = new() { Semidominator = rootNum, Original = new(1, null)};
        _originalByTimeIn[rootNum] = 1;
        foreach (var rootObj in roots
                     .Select(root => root.Object)
                     .Where(rootObj => !_vertices.ContainsKey(rootObj.Address)))
        {
            NumerateVerticesWithTimeInByDfs(rootObj, numerator);
            _vertices[rootObj].Parent = 1;
            _vertices[rootObj].Predecessors.Add(1);
        }

        var n = _vertices.Count;
        var vertices = new Vertex[_vertices.Count + 1];
        foreach (var (time, orig) in _originalByTimeIn)
        {
            vertices[time] = _vertices[orig];
        }
        
        var linkEvalTree = new UnbalancedLinkEvalTree(vertices, address => vertices[address].Semidominator);
        for (var i = n - 1; i > 1; i--)
        {
            var w = i;
            foreach (var u in vertices[w].Predecessors
                         .Select(linkEvalTree.Eval)
                         .Where(u => vertices[u].Semidominator < vertices[w].Semidominator))
            {
                vertices[w].Semidominator = vertices[u].Semidominator;
            }
            
            vertices[vertices[w].Semidominator].Bucket.Add(w);
            linkEvalTree.Link(vertices[w].Parent, w);
            foreach (var v in vertices[vertices[w].Parent].Bucket)
            {
                var u = linkEvalTree.Eval(v);
                vertices[v].Dominator = 
                    vertices[u].Semidominator < vertices[v].Semidominator 
                        ? u 
                        : vertices[w].Parent;
            }
        }
        
        for (var i = 2; i < n; i++)
        {
            if (vertices[i].Dominator != vertices[i].Semidominator)
            {
                vertices[i].Dominator = vertices[vertices[i].Dominator].Dominator;
            }
        }

        var dominatorTree = new List<(ulong Parent, ulong Child)>(n * 2);
        for (var i = 2; i < n; i++)
        {
            var vertex = vertices[i];
            
            dominatorTree.Add((vertices[vertex.Dominator].Original, vertex.Original));
        }

        return dominatorTree.ToLookup(kvp => kvp.Parent, kvp => kvp.Child);
    }
    
    private void NumerateVerticesWithTimeInByDfs(
        ClrObject currentObj,
        VerticesNumerator numerator)
    {
        var nodeNumber = numerator.AssignNewNumber();
        _vertices[currentObj] = new() { Semidominator = nodeNumber, Original = currentObj};
        
        _originalByTimeIn[nodeNumber] = currentObj;
        foreach (var child in currentObj.EnumerateReferences())
        {
            if (child.IsFree || child.IsNull) continue;
            
            if (!_vertices.ContainsKey(child))
            {
                NumerateVerticesWithTimeInByDfs(child, numerator);
                _vertices[child].Parent = nodeNumber;
            }
            
            _vertices[child].Predecessors.Add(nodeNumber);
        }
    }
    
    private class VerticesNumerator
    {
        public int Value { get; private set; }

        public int AssignNewNumber() => ++Value;
    }
    
    private class Vertex
    {
        public ClrObject Original;
        
        public int Dominator;

        public int Semidominator;

        public int Parent;
        
        public readonly List<int> Predecessors = new();
        
        public readonly List<int> Bucket = new();
    }
    
    private class UnbalancedLinkEvalTree
    {
        private readonly LinkEvalVertex[] _tree;
        private readonly Func<int, int> _evalFunction;
        
        public UnbalancedLinkEvalTree(Vertex[] vertices, Func<int, int> evalFunction)
        {
            _tree = new LinkEvalVertex[vertices.Length];
            for (var i = 1; i < vertices.Length; i++)
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
}