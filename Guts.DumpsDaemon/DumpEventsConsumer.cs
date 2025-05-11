using Guts.DumpAnalyzer;
using Guts.EventsModels;
using MassTransit;

namespace Guts.DumpsDaemon;

public class DumpEventsConsumer(IGutsServerClient serverClient) : IConsumer<UploadedDumpEvent>
{
    public async Task Consume(ConsumeContext<UploadedDumpEvent> context)
    {
        var dumpId = context.Message.DumpId;
        var dumpStream = await serverClient.GetDump(dumpId);
        var dump = Dump.Create(dumpId.ToString(), dumpStream);

        var report = new Report(
            GetTopTenTypesBySize(dump),
            GetTopTenTypesByRetainedSize(dump),
            GetTopTenBoxedTypesBySize(dump),
            GetGenerationsSizes(dump),
            GetUnhandledExceptions(dump));

        await serverClient.UploadReport(dumpId, report);
    }

    private static List<TypeAndSize> GetTopTenTypesBySize(Dump dump)
    {
        return dump.Graph
            .EnumerateByDfs()
            .Select(node => (node.Type.Name, node.Size))
            .Where(pair => pair.Name is not null)
            .OfType<(string Name, ulong Size)>()
            .GroupBy(pair => pair.Name)
            .Select(group => new TypeAndSize(
                group.Key,
                group
                    .Select(dominator => dominator.Size)
                    .Aggregate(0ul, (current, size) => current + size)))
            .OrderByDescending(group => group.Size)
            .Take(10)
            .ToList();
    }

    private static List<TypeAndSize> GetTopTenTypesByRetainedSize(Dump dump)
    {
        return dump
            .GetDominatorsTree()
            .Roots
            .Select(node => (node.DumpObject.Type.Name, node.RetainedSize))
            .Where(pair => pair.Name is not null)
            .OfType<(string Name, ulong Retained)>()
            .GroupBy(pair => pair.Name)
            .Select(group => new TypeAndSize(
                group.Key,
                group
                    .Select(dominator => dominator.Retained)
                    .Aggregate(0ul, (current, size) => current + size)))
            .OrderByDescending(group => group.Size)
            .Take(10)
            .ToList();
    }

    private static List<TypeAndSize> GetTopTenBoxedTypesBySize(Dump dump)
    {
        return dump.Graph
            .EnumerateByDfs()
            .Where(node => node.IsBoxed)
            .Select(node => (node.Type.Name, node.Size))
            .Where(pair => pair.Name is not null)
            .OfType<(string Name, ulong Size)>()
            .GroupBy(pair => pair.Name)
            .Select(group => new TypeAndSize(
                group.Key,
                group
                    .Select(dominator => dominator.Size)
                    .Aggregate(0ul, (current, size) => current + size)))
            .OrderByDescending(group => group.Size)
            .Take(10)
            .ToList();
    }

    private static List<GenerationSize> GetGenerationsSizes(Dump dump)
    {
        return dump.Graph.GenerationsSizes
            .Select(kvp => new GenerationSize(ToServerGeneration(kvp.Key), kvp.Value.TotalSize, kvp.Value.UsedSize))
            .ToList();

        static Generation ToServerGeneration(GenerationType generation)
        {
            return generation switch
            {
                GenerationType.Generation0 => Generation.Generation0,
                GenerationType.Generation1 => Generation.Generation1,
                GenerationType.Generation2 => Generation.Generation2,
                GenerationType.Large => Generation.Large,
                GenerationType.Pinned => Generation.Pinned,
                GenerationType.Frozen => Generation.Frozen,
                GenerationType.Unknown => Generation.Unknown,
                _ => throw new ArgumentOutOfRangeException(nameof(generation), generation, null)
            };
        }
    }

    private static List<UnhandledException> GetUnhandledExceptions(Dump dump)
    {
        return dump
            .GetUnhandledExceptions()
            .Exceptions
            .Select(exception => new UnhandledException(exception.ManagedThreadId, exception.StackTrace))
            .ToList();
    }
}