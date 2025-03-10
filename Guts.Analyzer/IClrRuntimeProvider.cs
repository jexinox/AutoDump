using Microsoft.Diagnostics.Runtime;

namespace Guts.Analyzer;

public interface IClrRuntimeProvider
{
    ClrRuntime GetCurrentDumpRuntime(); 
}