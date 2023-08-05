using Amazon;
using Amazon.Runtime;
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
    private readonly string _bucketName;

    public AwsS3StorageProvider(
        IOptions<AwsOptions> awsOptions,
        TemporaryStorageProvider temporaryStorageProvider
    )
    {
        ArgumentNullException.ThrowIfNull(awsOptions.Value.AccessKey);
        ArgumentNullException.ThrowIfNull(awsOptions.Value.SecretAccessKey);
        ArgumentNullException.ThrowIfNull(awsOptions.Value.BucketName);
        
        _temporaryStorageProvider = temporaryStorageProvider;
        _bucketName = awsOptions.Value.BucketName;
        _awsClient = new AmazonS3Client(
            awsAccessKeyId: awsOptions.Value.AccessKey,
            awsSecretAccessKey: awsOptions.Value.SecretAccessKey,
            RegionEndpoint.EUWest2
        );
    }

    public async Task<IReadOnlyList<SaveStorageObjectResult>> SaveObjectsAsync(IEnumerable<StorageObject> storageObjects,
        CancellationToken cancellationToken = default)
    {
        // start the multipart upload
        var results = new List<SaveStorageObjectResult>();
        foreach (var obj in storageObjects)
        {
            if (obj.TemporaryFileKey is null)
            {
                results.Add(new SaveStorageObjectResult(
                    StorageObjectId: obj.Id,
                    Success: false,
                    ErrorMessage: "Temporary File Key cannot be null"
                ));
                continue;
            }

            var objectKey = ConvertFileNameToObjectKey(obj);
            var temporaryFilePath = _temporaryStorageProvider.GetFilePath((Guid)obj.TemporaryFileKey);
            try
            {
                var putObjectResponse = await _awsClient.PutObjectAsync(new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = objectKey,
                    FilePath = temporaryFilePath,
                    ChecksumSHA1 = obj.FileHash
                }, cancellationToken);
                
                results.Add(new SaveStorageObjectResult(
                    StorageObjectId: obj.Id,
                    Success: true,
                    StoragePath: objectKey
                ));
            }
            catch (AmazonServiceException ex)
            {
                results.Add(new SaveStorageObjectResult(
                    StorageObjectId: obj.Id,
                    Success: false,
                    ErrorMessage: "An AWS exception occurred",
                    InnerException: ex
                ));
            }
        }

        return results;
    }

    public Task<Result<string>> GenerateDownloadLinkAsync(StorageObject storageObject)
    {
        var objectKey = ConvertFileNameToObjectKey(storageObject);
        var preSignedUrl = _awsClient.GetPreSignedURL(new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = objectKey,
            Expires = DateTime.Now.AddMinutes(10)
        });

        return preSignedUrl is not null
            ? Task.FromResult(Result.Success<string>(preSignedUrl))
            : Task.FromResult(Result.Failure<string>("Could not generate download url for the specified storage object."));
    }

    public Task<Result> DeleteObjectAsync(StorageObject storageObject)
    {
        throw new NotImplementedException();
    }


    private static string ConvertFileNameToObjectKey(StorageObject storageObject)
    {
        var fileNameHash = HashHelper.Sha1String(storageObject.ClientRelativePath).Replace("/", "_");
        return $"{storageObject.OwnerId}/{storageObject.BucketId}/{fileNameHash}";
    }
}