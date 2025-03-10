using Microsoft.Diagnostics.Runtime;

namespace Guts.Analyzer;

public class StubClrRuntimeProvider : IClrRuntimeProvider
{
    private readonly ClrRuntime _runtime;

    public StubClrRuntimeProvider(ClrRuntime runtime) => _runtime = runtime;

    public ClrRuntime GetCurrentDumpRuntime() => _runtime;
}