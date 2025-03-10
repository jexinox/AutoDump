using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.AbstractDac;

namespace Guts.Analyzer.Metrics;

// public class GenerationsSizeMetricProvider
// {
//     private readonly IClrRuntimeProvider _runtimeProvider;
//
//     public GenerationsSizeMetricProvider(IClrRuntimeProvider runtimeProvider)
//     {
//         _runtimeProvider = runtimeProvider;
//     }
//
//     public IReadOnlyDictionary<Generation, ulong> Get()
//     {
//         var runtime = _runtimeProvider.GetCurrentDumpRuntime();
//         
//         return runtime.Heap.Segments.Select(segment => segment.Kind)
//     }
//
//     private Generation GetGenerationBySegment(ClrSegment segment)
//     {
//         
//     }
// }