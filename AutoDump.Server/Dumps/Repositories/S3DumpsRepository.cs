using AutoDump.Server.Dumps.FeatureModels;
using Kontur.Results;
using Minio;
using Minio.DataModel.Args;

namespace AutoDump.Server.Dumps.Repositories;

public class S3DumpsRepository(IMinioClient minioClient) : IDumpsRepository
{
    private const string BucketName = "autodump-dumps";
    
    public async Task<Result<RepositoryUploadDumpError>> LoadDump(DumpId id, Dump dump)
    {
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(BucketName)
            .WithObject(id.Value.ToString())
            .WithStreamData(dump.Stream);

        await minioClient.PutObjectAsync(putObjectArgs);
        
        return Result.Succeed();
    }
}