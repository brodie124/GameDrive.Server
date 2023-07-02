using GameDrive.Server.Domain.Models;
using GameDrive.Server.Services.Repositories;
using Microsoft.AspNetCore.WebUtilities;

namespace GameDrive.Server.Services.Storage;

public class StorageService
{
    private readonly ILogger<StorageService> _logger;
    private readonly IStorageProvider _storageProvider;
    private readonly IStorageObjectRepository _storageObjectRepository;
    private readonly IBucketRepository _bucketRepository;

    public StorageService(
        ILogger<StorageService> logger,
        IStorageProvider storageProvider,
        IStorageObjectRepository storageObjectRepository,
        IBucketRepository bucketRepository
    )
    {
        _logger = logger;
        _storageProvider = storageProvider;
        _storageObjectRepository = storageObjectRepository;
        _bucketRepository = bucketRepository;
    }

    public async Task<StorageObject?> UploadFileAsync(SaveStorageObjectRequest saveStorageObjectRequest,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var bucket = (await _bucketRepository
                    .FindAsync(x => x.Id == saveStorageObjectRequest.BucketId))
                .FirstOrDefault();

            if (bucket is null)
            {
                bucket = new Bucket
                {
                    Id = saveStorageObjectRequest.BucketId,
                    Name = saveStorageObjectRequest.BucketName
                };

                await _bucketRepository.AddAsync(bucket);
            }

            var result = await _storageProvider.SaveObjectAsync(saveStorageObjectRequest, cancellationToken);
            if (!result.Success)
                return null;

            var existingStorageObject = (await _storageObjectRepository
                    .FindAsync(x => x.BucketId == bucket.Id && x.ClientRelativePath == saveStorageObjectRequest.GdFilePath))
                    .FirstOrDefault();
            if (existingStorageObject is not null)
            {
                existingStorageObject.MarkForDeletion();
                await _storageObjectRepository.RemoveAsync(existingStorageObject);
            }

            var storageObject = result.StorageObject!;
            storageObject.BucketId = bucket.Id;

            await _storageObjectRepository.AddAsync(storageObject);
            await _storageObjectRepository.SaveChangesAsync();
            return storageObject;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred whilst uploading file (\'{FileName}\')",
                saveStorageObjectRequest.GdFilePath);
            return null;
        }
    }
}