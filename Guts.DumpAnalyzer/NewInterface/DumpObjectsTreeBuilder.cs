using System.Diagnostics.CodeAnalysis;
using Microsoft.Diagnostics.Runtime;

namespace Guts.DumpAnalyzer;

internal class DumpObjectsTreeBuilder
{
    private readonly Dictionary<ulong, DumpObject> _builtObjects = new();
    
    private int _globalNodeNumber = DumpDominatorsTreeBuilder.FakeRoot; 
        
    public DumpObjectsTree Build(ClrHeap heap)
    {
        var dumpObjectRoots = new List<DumpObject>();
        foreach (var root in heap.EnumerateRoots())
        {
            if (TryGetDumpObject(root.Object, out var rootDumpObject))
            {
                dumpObjectRoots.Add(rootDumpObject);
            }
        }
            
        return new(dumpObjectRoots, _builtObjects.Count);
    }
    
    private bool TryGetDumpObject(ClrObject currentObject, [NotNullWhen(true)] out DumpObject? dumpObject)
    {
        if (currentObject.IsFree || currentObject.IsNull)
        {
            dumpObject = null;
            return false;
        }
            
        if (_builtObjects.TryGetValue(currentObject, out var builtDumpObject))
        {
            dumpObject = builtDumpObject;
            return true;
        }
    
        var currentObjectReferences = new List<DumpObject>();
        var currentNodeNumber = ++_globalNodeNumber;
        var currentDumpObj = new DumpObject(currentObject, currentObjectReferences, currentNodeNumber);
        _builtObjects[currentObject] = currentDumpObj;
    
        foreach (var reference in currentObject.EnumerateReferences())
        {
            if (TryGetDumpObject(reference, out var referencedDumpObject))
            {
                currentObjectReferences.Add(referencedDumpObject);
            }
        }
    
        dumpObject = currentDumpObj;
        return true;
    }
}