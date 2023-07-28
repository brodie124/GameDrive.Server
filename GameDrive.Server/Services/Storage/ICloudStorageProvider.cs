using GameDrive.Server.Domain.Models;

namespace GameDrive.Server.Services.Storage;

public interface ICloudStorageProvider
{
    Task<SaveStorageObjectResult> SaveObjectAsync(SaveStorageObjectRequest saveRequest, CancellationToken cancellationToken = default);
    Task<DownloadStorageObjectResult> GenerateDownloadLinkAsync(StorageObject storageObject);
    Task<DeleteStorageObjectResult> DeleteObjectAsync(StorageObject storageObject);
}

public record SaveStorageObjectResult(
    bool Success,
    StorageObject? StorageObject
);

public record DeleteStorageObjectResult(
    bool Success
);

public record DownloadStorageObjectResult(
    bool Success,
    string DownloadUrl
);

public record SaveStorageObjectRequest(
    int OwnerId,
    string BucketId,
    string BucketName,
    string GdFilePath,
    string FileHash,
    DateTime FileCreatedDate,
    DateTime FileLastModifiedDate,
    Stream SourceStream
);