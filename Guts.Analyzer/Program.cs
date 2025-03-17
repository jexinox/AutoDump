using Guts.Analyzer;
using Guts.Analyzer.DataProviders;
using Microsoft.Diagnostics.Runtime;

var dump = DataTarget.LoadDump("./Dumps/test-dump-net6s.dmp");

var clr = dump.ClrVersions[0].CreateRuntime();

// foreach (var heap in clr.EnumerateClrNativeHeaps())
// {
//     Console.WriteLine(heap.Kind);
//     Console.WriteLine(heap.GCHeap);
// }
var clrProvider = new StubClrRuntimeProvider(clr);
// var objectsDistributionDataProvider = new ObjectsDistributionByGenerationsDataProvider(clrProvider);
var objectsTreeProvider = new ObjectsTreeProvider(clrProvider);
var fragmentationDataProvider = new GenerationsSizesDataProvider(clrProvider, objectsTreeProvider);

Console.WriteLine($"Is server: {clr.Heap.IsServer}");
Console.WriteLine($"Uses regions: {clr.Heap.Segments.Any(segment => segment.Kind is GCSegmentKind.Generation0 or GCSegmentKind.Generation1)}");

foreach (var (generation, fragmentation) in fragmentationDataProvider.Get())
{
    Console.WriteLine($"{generation}, Total (B): {fragmentation.Total}, Total (MB): {fragmentation.Total / 1024d / 1024d}, Used (B): {fragmentation.Used} Used (KB): {fragmentation.Used / 1024d}, Used (MB): {fragmentation.Used / 1024d / 1024d}");
}
