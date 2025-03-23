using Microsoft.Diagnostics.Runtime;

namespace Guts.Analyzer.DataProviders;

public class DominatorTreeBuilder
{
    private readonly Dictionary<ulong, Vertex> _vertices = new();
    private readonly Dictionary<int, ulong> _originalByTimeIn = new();
        
    public IReadOnlyDictionary<ulong, List<ulong>> Build(ClrHeap heap)
    {
        var roots = heap.EnumerateRoots().ToList();
        var numerator = new VerticesNumerator();
        foreach (var root in roots)
        {
            var rootObj = root.Object;
            if (!_vertices.ContainsKey(rootObj.Address))
            {
                NumerateVerticesWithTimeInByDfs(rootObj, numerator);
            }
        }
        
        var n = _vertices.Count;
        var linkEvalTree = new UnbalancedLinkEvalTree(address => _vertices[address].Semidominator);
        for (var i = n - 1; i > 1; i--)
        {
            var w = _originalByTimeIn[i];
            foreach (var u in _vertices[w].Predecessors
                         .Select(linkEvalTree.Eval)
                         .Where(u => _vertices[u].Semidominator < _vertices[w].Semidominator))
            {
                _vertices[w].Semidominator = _vertices[u].Semidominator;
            }
            _vertices[_originalByTimeIn[_vertices[w].Semidominator]].Bucket.Add(w);
            linkEvalTree.Link(_vertices[w].Parent, w);
            foreach (var v in _vertices[_vertices[w].Parent].Bucket)
            {
                var u = linkEvalTree.Eval(v);
                _vertices[v].Dominator = 
                    _vertices[u].Semidominator < _vertices[v].Semidominator 
                        ? u 
                        : _vertices[w].Parent;
            }
        }
        
        for (var i = 2; i < n; i++)
        {
            var w = _originalByTimeIn[i];
            if (_vertices[w].Dominator != _originalByTimeIn[_vertices[w].Semidominator])
            {
                _vertices[w].Dominator = _vertices[_vertices[w].Dominator].Dominator;
            }
        }

        _vertices[_originalByTimeIn[1]].Dominator = 0;

        var dominatorTree = new Dictionary<ulong, List<ulong>>();
        foreach (var (obj, vertex) in _vertices)
        {
            if (dominatorTree.TryGetValue(vertex.Dominator, out var dominatorRefs))
            {
                dominatorRefs.Add(obj);
            }
            else
            {
                dominatorTree[vertex.Dominator] = [obj];
            }
        }

        return dominatorTree;
    }
    
    private void NumerateVerticesWithTimeInByDfs(
        ClrObject currentObj,
        VerticesNumerator numerator)
    {
        var nodeNumber = numerator.AssignNewNumber();
        if (_vertices.TryGetValue(currentObj, out var vertex))
        {
            vertex.Semidominator = nodeNumber;    
        }
        else
        {
            _vertices[currentObj] = new() { Semidominator = nodeNumber};
        }
        
        _originalByTimeIn[nodeNumber] = currentObj;
        foreach (var child in currentObj.EnumerateReferences())
        {
            if (child.IsFree) continue;
            
            if (!_vertices.ContainsKey(child))
            {
                NumerateVerticesWithTimeInByDfs(child, numerator);
                _vertices[child].Parent = currentObj;
            }
            
            _vertices[child].Predecessors.Add(currentObj);
        }
    }
    
    private class VerticesNumerator
    {
        public int Value { get; private set; }

        public int AssignNewNumber() => ++Value;
    }
    
    private class Vertex
    {
        public ulong Dominator;

        public int Semidominator;

        public ulong Parent;
        
        public readonly List<ulong> Predecessors = new();
        
        public readonly List<ulong> Bucket = new();
    }
    
    private class UnbalancedLinkEvalTree
    {
        private readonly Dictionary<ulong, LinkEvalVertex> _tree = new();
        private readonly Func<ulong, int> _evalFunction;
        
        public UnbalancedLinkEvalTree(Func<ulong, int> evalFunction)
        {
            _evalFunction = evalFunction;
        }
        
        public ulong Eval(ulong v)
        {
            var vertex = GetOrCreateVertex(v);
            if (vertex.Ancestor.Object == 0)
            {
                return v;
            }
    
            Compress(vertex);
            return vertex.Label;
        }
    
        public void Link(ulong v, ulong w)
        {
            var linkEvalV = GetOrCreateVertex(v);
            var linkEvalW = GetOrCreateVertex(w);
            linkEvalW.Ancestor = linkEvalV;
        }
        
        private void Compress(LinkEvalVertex v)
        {
            if (v.Ancestor.Ancestor.Object == 0)
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

        private LinkEvalVertex GetOrCreateVertex(ulong address)
        {
            if (_tree.TryGetValue(address, out var vertex))
            {
                return vertex;
            }
            
            return _tree[address] = new()
            {
                Object = address,
                Label = address,
                Ancestor = new() { Object = 0 }
            };
        }
        
        private class LinkEvalVertex
        {
            public ulong Object;
    
            public ulong Label;
            
            public LinkEvalVertex Ancestor = null!;
        }
    }
}