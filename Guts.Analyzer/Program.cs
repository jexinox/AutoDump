using Guts.Analyzer;
using Guts.Analyzer.DataProviders;
using Guts.Analyzer.Statistics;
using Microsoft.Diagnostics.Runtime;

var dump = DataTarget.LoadDump("./Dumps/test-dump-net6s.dmp");

var clr = dump.ClrVersions[0].CreateRuntime();

var clrProvider = new StubClrRuntimeProvider(clr);
var objectsTreeProvider = new ObjectsTreeProvider(clrProvider);
var objectsDistributionDataProvider = new ObjectsDistributionByGenerationsDataProvider(clrProvider, objectsTreeProvider);
var generationsSizesDataProvider = new GenerationsSizesStatProvider(objectsDistributionDataProvider);

Console.WriteLine($"Is server: {clr.Heap.IsServer}");
Console.WriteLine($"Uses regions: {clr.Heap.Segments.Any(segment => segment.Kind is GCSegmentKind.Generation0 or GCSegmentKind.Generation1)}");

foreach (var (generation, size) in generationsSizesDataProvider.Get())
{
    Console.WriteLine(
        $"{generation}, Total (B): {size.Total}, Total (MB): {size.Total / 1024d / 1024d}, " +
        $"Used (B): {size.Used} Used (KB): {size.Used / 1024d}, Used (MB): {size.Used / 1024d / 1024d}");
}
