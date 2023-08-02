using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using GameDrive.Server.Domain.Models;
using GameDrive.Server.Utilities;

namespace GameDrive.Server.Services.Storage;

public class AwsS3StorageProvider : ICloudStorageProvider
{
    private readonly TemporaryStorageProvider _temporaryStorageProvider;
    private const string BucketName = "gamedrive-user-saves";

    public AwsS3StorageProvider(
        TemporaryStorageProvider temporaryStorageProvider
    )
    {
        _temporaryStorageProvider = temporaryStorageProvider;
    }
    
    public async Task<Result> SaveObjectsAsync(IEnumerable<StorageObject> storageObjects, CancellationToken cancellationToken = default)
    {
        // TODO: move to IOptions
        var client = new AmazonS3Client(
            awsAccessKeyId: "xxx",
            awsSecretAccessKey: "xxx",
            RegionEndpoint.EUWest2
        );
        
        // start the multipart upload
        foreach (var obj in storageObjects)
        {
            if (obj.TemporaryFileKey is null)
            {
                throw new InvalidOperationException("Temporary File Key cannot be null");
                continue;
            }
            
            var objectKey = FileNameToObjectKey(obj);
            var temporaryFilePath = _temporaryStorageProvider.GetFilePath((Guid) obj.TemporaryFileKey);
            // TODO: handles errors & exceptions
            var objects = await client.PutObjectAsync(new PutObjectRequest
            {
                BucketName = BucketName,
                Key = objectKey,
                FilePath = temporaryFilePath,
                ChecksumSHA1 = obj.FileHash
            }, cancellationToken);
        }


        return Result.Success();
    }

    public Task<Result<string>> GenerateDownloadLinkAsync(StorageObject storageObject)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteObjectAsync(StorageObject storageObject)
    {
        throw new NotImplementedException();
    }
    
    
    
    
    
    

    private string FileNameToObjectKey(StorageObject storageObject)
    {
        var fileNameHash = HashHelper.Sha1String(storageObject.ClientRelativePath);
        return $"{storageObject.OwnerId}/{fileNameHash}";
    }
}