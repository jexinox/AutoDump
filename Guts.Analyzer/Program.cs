using Microsoft.Diagnostics.Runtime;

var dump = DataTarget.LoadDump("./test-dump.dmp");

var clr = dump.ClrVersions[0].CreateRuntime();

// foreach (var heap in clr.EnumerateClrNativeHeaps())
// {
//     Console.WriteLine(heap.Kind);
//     Console.WriteLine(heap.GCHeap);
// }
var objectsGenerations = clr.Heap
    .EnumerateObjects()
    .Select(obj => obj.Address)
    .Select(address =>
    {
        var segment = clr.Heap.GetSegmentByAddress(address);
        return segment?.GetGeneration(address);
    })
    .SelectMany(gen => gen is null ? [] : new[] { gen.Value })
    .CountBy(gen => gen)
    .ToDictionary();

foreach (var kvp in objectsGenerations)
{
    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
}

