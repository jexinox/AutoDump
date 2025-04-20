using Guts.Server.Dumps.FeatureModels;
using Guts.Server.Dumps.Repositories.Metadata;
using Kontur.Results;
using Minio;
using Minio.DataModel.Args;

namespace Guts.Server.Dumps.Repositories.Dumps;

public class S3DumpsRepository(IMinioClient minioClient) : IDumpsRepository
{
    private const string BucketName = "guts-dumps";
    
    public async Task<Result<RepositoryUploadDumpMetadataError>> LoadDump(DumpId id, Dump dump)
    {
        var putObjectArgs = new PutObjectArgs()
            .WithBucket(BucketName)
            .WithObject(id.Value.ToString())
            .WithStreamData(dump.Stream);

        await minioClient.PutObjectAsync(putObjectArgs);
        
        return Result.Succeed();
    }
}