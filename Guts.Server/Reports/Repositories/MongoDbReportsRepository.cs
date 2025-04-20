using Guts.Server.Reports.FeatureModels;
using Kontur.Results;
using MongoDB.Driver;

namespace Guts.Server.Reports.Repositories;

public class MongoDbReportsRepository(IMongoCollection<MongoReport> collection) : IReportsRepository
{
    public async Task<Result<RepositoryUploadReportError>> Upload(Report report)
    {
        var mongoReport = ToMongoReport(report);
        
        await collection.InsertOneAsync(mongoReport);
        
        return Result.Succeed();
    }

    public async Task<Result<RepositorySearchReportError, IReadOnlyCollection<Report>>> Search(DumpId dumpId)
    {
        var searchResult = await collection.Find(report => report.DumpId == dumpId.Value).ToListAsync();
        
        return searchResult.Select(ToReport).ToArray();
    }

    private static MongoReport ToMongoReport(Report report)
    {
        return new()
        {
            DumpId = report.DumpId.Value,
            TypesTopBySize = report.TypesTopBySize.Select(x => new MongoTypeAndSize(x.Type, x.Size)).ToArray(),
            TypesTopByRetainedSize = report.TypesTopByRetainedSize
                .Select(x => new MongoTypeAndSize(x.Type, x.Size))
                .ToArray(),
            BoxedTypesTopBySize = report.BoxedTypesTopBySize
                .Select(x => new MongoTypeAndSize(x.Type, x.Size))
                .ToArray(),
            GenerationsSizes = report.GenerationsSizes
                .Select(x => new MongoGenerationSize(ToMongoGeneration(x.Generation), x.TotalSize, x.UsedSize))
                .ToArray(),
            Exceptions = report.Exceptions
                .Select(x => new MongoUnhandledException(x.ManagedThreadId, x.StackTrace))
                .ToArray(),
        };
    }

    private static MongoGeneration ToMongoGeneration(Generation generation)
    {
        return generation switch
        {
            Generation.Generation0 => MongoGeneration.Generation0,
            Generation.Generation1 => MongoGeneration.Generation1,
            Generation.Generation2 => MongoGeneration.Generation2,
            Generation.Large => MongoGeneration.Large,
            Generation.Pinned => MongoGeneration.Pinned,
            Generation.Frozen => MongoGeneration.Frozen,
            Generation.Unknown => MongoGeneration.Unknown,
            _ => throw new ArgumentOutOfRangeException(nameof(generation), generation, null)
        };
    }

    private static Report ToReport(MongoReport mongoReport)
    {
        return new(
            new(mongoReport.DumpId),
            mongoReport.TypesTopBySize.Select(x => new TypeAndSize(x.Type, x.Size)).ToArray(),
            mongoReport.TypesTopByRetainedSize.Select(x => new TypeAndSize(x.Type, x.Size)).ToArray(),
            mongoReport.BoxedTypesTopBySize.Select(x => new TypeAndSize(x.Type, x.Size)).ToArray(),
            mongoReport.GenerationsSizes
                .Select(x => new GenerationSize(ToGeneration(x.Generation), x.TotalSize, x.UsedSize))
                .ToArray(),
            mongoReport.Exceptions.Select(x => new UnhandledException(x.ManagedThreadId, x.StackTrace)).ToArray());
    }
    
    private static Generation ToGeneration(MongoGeneration generation)
    {
        return generation switch
        {
            MongoGeneration.Generation0 => Generation.Generation0,
            MongoGeneration.Generation1 => Generation.Generation1,
            MongoGeneration.Generation2 => Generation.Generation2,
            MongoGeneration.Large => Generation.Large,
            MongoGeneration.Pinned => Generation.Pinned,
            MongoGeneration.Frozen => Generation.Frozen,
            MongoGeneration.Unknown => Generation.Unknown,
            _ => throw new ArgumentOutOfRangeException(nameof(generation), generation, null)
        };
    }
}