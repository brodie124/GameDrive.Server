using GameDrive.Server.Domain.Models;
using GameDrive.Server.Services.Repositories;

namespace GameDrive.Server.Services.Storage;

public class StorageService
{
    private readonly ILogger<StorageService> _logger;
    private readonly ICloudStorageProvider _cloudStorageProvider;
    private readonly IStorageObjectRepository _storageObjectRepository;
    private readonly IBucketRepository _bucketRepository;

    public StorageService(
        ILogger<StorageService> logger,
        ICloudStorageProvider cloudStorageProvider,
        IStorageObjectRepository storageObjectRepository,
        IBucketRepository bucketRepository
    )
    {
        _logger = logger;
        _cloudStorageProvider = cloudStorageProvider;
        _storageObjectRepository = storageObjectRepository;
        _bucketRepository = bucketRepository;
    }

    public async Task<StorageObject?> UploadFileAsync(
        SaveStorageObjectRequest saveStorageObjectRequest,
        bool saveChanges = true, // TODO: Revisit this way of delaying saving
        CancellationToken cancellationToken = default
    )
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

            var result = await _cloudStorageProvider.SaveObjectAsync(saveStorageObjectRequest, cancellationToken);
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
            
            if(saveChanges)
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