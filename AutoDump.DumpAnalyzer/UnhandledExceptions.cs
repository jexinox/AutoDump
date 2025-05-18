namespace AutoDump.DumpAnalyzer;

public class UnhandledExceptions
{
    public UnhandledExceptions(IEnumerable<UnhandledException> exceptions)
    {
        Exceptions = exceptions;
    }

    public IEnumerable<UnhandledException> Exceptions { get; }
}

public record UnhandledException(int ManagedThreadId, string StackTrace);