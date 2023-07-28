using CSharpFunctionalExtensions;
using GameDrive.Server.Domain.Models;

namespace GameDrive.Server.Services.Storage;

public interface ICloudStorageProvider
{
    Task<Result<IReadOnlyList<StorageObject>>> SaveObjectsAsync(IEnumerable<SaveStorageObjectRequest> request, CancellationToken cancellationToken = default);
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
    Stream SourceStream
);