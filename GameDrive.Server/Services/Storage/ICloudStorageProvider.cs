using CSharpFunctionalExtensions;
using GameDrive.Server.Domain.Models;

namespace GameDrive.Server.Services.Storage;

public interface ICloudStorageProvider
{
    Task<Result> SaveObjectsAsync(
        IEnumerable<StorageObject> storageObjects,
        CancellationToken cancellationToken = default
    );
    Task<Result<string>> GenerateDownloadLinkAsync(StorageObject storageObject);
    Task<Result> DeleteObjectAsync(StorageObject storageObject);
}

public record SaveStorageObjectRequest(
    int OwnerId,
    string BucketId,
    string BucketName,
    string GdFilePath,
    string FileHash,
    DateTime FileCreatedDate,
    DateTime FileLastModifiedDate,
    Guid TemporaryFileKey
);