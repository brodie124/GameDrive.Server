using GameDrive.Server.Domain.Models;
using GameDrive.Server.Services.Repositories;
using Microsoft.AspNetCore.WebUtilities;

namespace GameDrive.Server.Services.Storage;

public class StorageService
{
    private readonly ILogger<StorageService> _logger;
    private readonly IStorageProvider _storageProvider;
    private readonly StorageObjectRepository _storageObjectRepository;
    private readonly BucketRepository _bucketRepository;

    public StorageService(
        ILogger<StorageService> logger,
        IStorageProvider storageProvider,
        StorageObjectRepository storageObjectRepository,
        BucketRepository bucketRepository
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