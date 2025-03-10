using Guts.Analyzer;
using Guts.Analyzer.Metrics;
using Microsoft.Diagnostics.Runtime;

var dump = DataTarget.LoadDump("./test-dump.dmp");

var clr = dump.ClrVersions[0].CreateRuntime();

// foreach (var heap in clr.EnumerateClrNativeHeaps())
// {
//     Console.WriteLine(heap.Kind);
//     Console.WriteLine(heap.GCHeap);
// }
var clrProvider = new StubClrRuntimeProvider(clr);
var objectsGenerationsMetricProvider = new ObjectsByGenerationsDistributionMetricProvider(clrProvider);

Console.WriteLine(clr.Heap.Segments.Any(segment => segment.Kind is GCSegmentKind.Generation0 or GCSegmentKind.Generation1));

foreach (var kvp in objectsGenerationsMetricProvider.Get())
{
    Console.WriteLine($"{kvp.Key}: {kvp.Count()}");
}
