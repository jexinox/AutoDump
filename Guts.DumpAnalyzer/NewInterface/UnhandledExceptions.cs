namespace Guts.DumpAnalyzer;

public class UnhandledExceptions
{
    public UnhandledExceptions(IReadOnlyList<UnhandledException> exceptions)
    {
        Exceptions = exceptions;
    }

    public IReadOnlyList<UnhandledException> Exceptions { get; }
}

public record UnhandledException(int ManagedThreadId, string StackTrace);