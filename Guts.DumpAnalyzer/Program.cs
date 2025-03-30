using System.Diagnostics;
using Guts.DumpAnalyzer;
using Guts.DumpAnalyzer.DataProviders;
using Microsoft.Diagnostics.Runtime;

// var stopwatch = Stopwatch.StartNew();
// var dataTarget = DataTarget.LoadDump("./Dumps/test-dump-net6s.dmp");
// var runtime = dataTarget.ClrVersions[0].CreateRuntime();
// var objectsTreeProvider = new ObjectsTreeFactory(new StubClrRuntimeProvider(runtime));
// Console.WriteLine(objectsTreeProvider.GetTree().Roots.Count);
//
// stopwatch.Stop();
// Console.WriteLine(stopwatch.ElapsedMilliseconds);

var stopwatch = Stopwatch.StartNew();
var dump = Dump.Create("./Dumps/test-dump-net6s.dmp");

Console.WriteLine(dump.Tree.Count);
Console.WriteLine(
    string.Join(
        Environment.NewLine,
        dump
            .GetDominatorsTree()
            .Roots
            .Select(node => (node.DumpObject.Object.Type?.Name, node.RetainedSize))
            .Where(pair => pair.Name is not null)
            .OfType<(string Name, ulong Retained)>()
            .GroupBy(pair => pair.Name)
            .Select(group => (
                Name: group.Key,
                Retained: group
                    .Select(dominator => dominator.Retained)
                    .Aggregate(0ul, (current, size) => current + size)))
            .OrderByDescending(group => group.Retained).Take(10)));

stopwatch.Stop();
Console.WriteLine(stopwatch.ElapsedMilliseconds);