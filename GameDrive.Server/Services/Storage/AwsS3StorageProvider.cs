using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using CSharpFunctionalExtensions;
using GameDrive.Server.Domain.Models;
using GameDrive.Server.Models.Options;
using GameDrive.Server.Utilities;
using Microsoft.Extensions.Options;

namespace GameDrive.Server.Services.Storage;

public class AwsS3StorageProvider : ICloudStorageProvider
{
    private readonly TemporaryStorageProvider _temporaryStorageProvider;
    private readonly IAmazonS3 _awsClient;
    private const string BucketName = "gamedrive-user-saves";

    public AwsS3StorageProvider(
        IOptions<AwsOptions> awsOptions,
        TemporaryStorageProvider temporaryStorageProvider
    )
    {
        ArgumentNullException.ThrowIfNull(awsOptions.Value.AccessKey);
        ArgumentNullException.ThrowIfNull(awsOptions.Value.SecretAccessKey);
        
        _temporaryStorageProvider = temporaryStorageProvider;
        _awsClient = new AmazonS3Client(
            awsAccessKeyId: awsOptions.Value.AccessKey,
            awsSecretAccessKey: awsOptions.Value.SecretAccessKey,
            RegionEndpoint.EUWest2
        );
    }

    public async Task<Result> SaveObjectsAsync(IEnumerable<StorageObject> storageObjects,
        CancellationToken cancellationToken = default)
    {
        // start the multipart upload
        foreach (var obj in storageObjects)
        {
            if (obj.TemporaryFileKey is null)
            {
                throw new InvalidOperationException("Temporary File Key cannot be null");
                continue;
            }

            var objectKey = FileNameToObjectKey(obj);
            var temporaryFilePath = _temporaryStorageProvider.GetFilePath((Guid)obj.TemporaryFileKey);
            // TODO: handles errors & exceptions
            var objects = await _awsClient.PutObjectAsync(new PutObjectRequest
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