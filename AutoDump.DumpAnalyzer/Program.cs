using AutoDump.DumpAnalyzer;

var dump = Dump.Create("./Dumps/test-dump-net6s.dmp");

Console.WriteLine("Top-10 types by retained size");
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

Console.WriteLine();

Console.WriteLine("Top-10 types by size");
Console.WriteLine(
    string.Join(
        Environment.NewLine,
        dump.Graph
            .EnumerateByDfs()
            .Select(node => (node.Type.Name, node.Size))
            .Where(pair => pair.Name is not null)
            .OfType<(string Name, ulong Size)>()
            .GroupBy(pair => pair.Name)
            .Select(group => (
                Name: group.Key,
                Size: group
                    .Select(dominator => dominator.Size)
                    .Aggregate(0ul, (current, size) => current + size)))
            .OrderByDescending(group => group.Size)
            .Take(10)));

Console.WriteLine();

Console.WriteLine("Generations sizes");
Console.WriteLine(string.Join(Environment.NewLine, dump.Graph.GenerationsSizes));

Console.WriteLine();

Console.WriteLine("Top-10 boxed types by size");
Console.WriteLine(
    string.Join(
        Environment.NewLine,
        dump.Graph
            .EnumerateByDfs()
            .Where(node => node.IsBoxed)
            .Select(node => (node.Type.Name, node.Size))
            .Where(pair => pair.Name is not null)
            .OfType<(string Name, ulong Size)>()
            .GroupBy(pair => pair.Name)
            .Select(group => (
                Name: group.Key,
                Size: group
                    .Select(dominator => dominator.Size)
                    .Aggregate(0ul, (current, size) => current + size)))
            .OrderByDescending(group => group.Size)
            .Take(10)));

Console.WriteLine("Unhandled exceptions");
Console.WriteLine(string.Join(Environment.NewLine, dump.GetUnhandledExceptions().Exceptions));
