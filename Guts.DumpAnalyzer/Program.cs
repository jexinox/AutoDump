using System.Diagnostics;
using Guts.DumpAnalyzer;

var stopwatch = Stopwatch.StartNew();
var dump = Dump.Create("./Dumps/test-dump-net6s.dmp");

Console.WriteLine(dump.Tree.Count);
Console.WriteLine(
    string.Join(
        Environment.NewLine,
        dump
            .GetDominatorsTree()
            .Roots
            .Select(node => (node.DumpObject.Type.Name, node.RetainedSize))
            .Where(pair => pair.Name is not null)
            .OfType<(string Name, ulong Retained)>()
            .GroupBy(pair => pair.Name)
            .Select(group => (
                Name: group.Key,
                Retained: group
                    .Select(dominator => dominator.Retained)
                    .Aggregate(0ul, (current, size) => current + size)))
            .OrderByDescending(group => group.Retained)
            .Take(10)));

Console.WriteLine(string.Join(Environment.NewLine, dump.Tree.GenerationsSizes));

stopwatch.Stop();
Console.WriteLine(stopwatch.ElapsedMilliseconds);