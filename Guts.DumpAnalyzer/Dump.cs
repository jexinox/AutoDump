using Microsoft.Diagnostics.Runtime;

namespace Guts.DumpAnalyzer;

public class Dump
{
    private readonly ClrRuntime _runtime;

    private Dump(ClrRuntime runtime, DumpObjectsGraph graph)
    {
        _runtime = runtime;
        Graph = graph;
    }

    public static Dump Create(string pathToDump)
    {
        var dataTarget = DataTarget.LoadDump(pathToDump);
        var runtime = dataTarget.ClrVersions[0].CreateRuntime();
        return new(runtime, DumpObjectsGraph.Create(runtime.Heap));
    }

    public DumpObjectsGraph Graph { get; }
    
    public DumpDominatorsTree GetDominatorsTree() => DumpDominatorsTree.Create(Graph);

    public UnhandledExceptions GetUnhandledExceptions()
    {
        return new(
            _runtime
                .Threads
                .Where(thread => thread.CurrentException is not null)
                .Select(thread => new UnhandledException(
                    thread.ManagedThreadId,
                    thread.CurrentException!.ToString())));
    }
}