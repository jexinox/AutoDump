using Microsoft.Diagnostics.Runtime;

namespace Guts.Analyzer.Statistics;

public class UnhandledExceptionsProvider
{
    public IReadOnlyList<(int ThreadId, string Exception)> Get(ClrRuntime runtime)
    {
        return runtime
            .Threads
            .Where(thread => thread.CurrentException is not null)
            .Select(thread => (
                thread.ManagedThreadId,
                thread.CurrentException!.ToString()))
            .ToList();
    }
}