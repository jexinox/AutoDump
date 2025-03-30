using Microsoft.Diagnostics.Runtime;

namespace Guts.DumpAnalyzer;

public class Dump
{
    private readonly ClrRuntime _runtime;

    private Dump(ClrRuntime runtime, DumpObjectsTree tree)
    {
        _runtime = runtime;
        Tree = tree;
    }

    public static Dump Create(string pathToDump)
    {
        var dataTarget = DataTarget.LoadDump(pathToDump);
        var runtime = dataTarget.ClrVersions[0].CreateRuntime();
        return new(runtime, DumpObjectsTree.Create(runtime.Heap));
    }

    public DumpObjectsTree Tree { get; }
    
    public DumpDominatorsTree GetDominatorsTree() => DumpDominatorsTree.Create(Tree);

    // public Generations GetGenerations();

    public UnhandledExceptions GetUnhandledExceptions()
    {
        return new(
            _runtime
                .Threads
                .Where(thread => thread.CurrentException is not null)
                .Select(thread => new UnhandledException(
                    thread.ManagedThreadId,
                    thread.CurrentException!.ToString()))
                .ToList());
    }
}