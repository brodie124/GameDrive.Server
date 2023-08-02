using CSharpFunctionalExtensions;
using GameDrive.Server.Domain.Models;
using GameDrive.Server.Services.Repositories;

namespace GameDrive.Server.Services.Storage;

public class StorageService
{
    private readonly ILogger<StorageService> _logger;
    private readonly ICloudStorageProvider _cloudStorageProvider;
    private readonly IStorageObjectRepository _storageObjectRepository;
    private readonly IBucketRepository _bucketRepository;
    private readonly TemporaryStorageProvider _temporaryStorageProvider;

    public StorageService(
        ILogger<StorageService> logger,
        ICloudStorageProvider cloudStorageProvider,
        IStorageObjectRepository storageObjectRepository,
        IBucketRepository bucketRepository,
        TemporaryStorageProvider temporaryStorageProvider
    )
    {
        _logger = logger;
        _cloudStorageProvider = cloudStorageProvider;
        _storageObjectRepository = storageObjectRepository;
        _bucketRepository = bucketRepository;
        _temporaryStorageProvider = temporaryStorageProvider;
    }

    public async Task SaveFilesAsync(
        IEnumerable<SaveStorageObjectRequest> saveRequests,
        CancellationToken cancellationToken = default    
    )
    {
        foreach (var request in saveRequests)
        {
            var temporaryFileExists = await _temporaryStorageProvider.HasFileAsync(request.TemporaryFileKey);
            if (!temporaryFileExists)
                throw new InvalidOperationException("Attempted to save file but it no longer exists.");

            var storageObject = ConvertSaveRequestToStorageObject(request, request.TemporaryFileKey);
            var bucket = await GetOrCreateBucketAsync(new Bucket
            {
                Id = request.BucketId,
                Name = request.BucketName
            });

            var existingStorageObject = await GetExistingStorageObject(bucket.Id, request.GdFilePath);
            if (existingStorageObject is not null)
            {
                existingStorageObject.MarkForDeletion();
                await _storageObjectRepository.RemoveAsync(existingStorageObject);
            }
            
            await _storageObjectRepository.AddAsync(storageObject);
            
            // TODO: pass the object off to the Replication Queue
        }

        await _storageObjectRepository.SaveChangesAsync();
    }


    // public async Task<StorageObject?> UploadFileAsync(
    //     SaveStorageObjectRequest saveStorageObjectRequest,
    //     bool saveChanges = true, // TODO: Revisit this way of delaying saving
    //     CancellationToken cancellationToken = default
    // )
    // {
    //     try
    //     {
    //         var bucket = await GetOrCreateBucketAsync(new Bucket
    //         {
    //             Id = saveStorageObjectRequest.BucketId,
    //             Name = saveStorageObjectRequest.BucketName
    //         });
    //         var result = await _cloudStorageProvider.SaveObjectsAsync(new[] { saveStorageObjectRequest }, cancellationToken);
    //         if (result.IsFailure)
    //             return null;
    //
    //         var existingStorageObject = (await _storageObjectRepository
    //                 .FindAsync(x => x.BucketId == bucket.Id && x.ClientRelativePath == saveStorageObjectRequest.GdFilePath))
    //                 .FirstOrDefault();
    //         if (existingStorageObject is not null)
    //         {
    //             existingStorageObject.MarkForDeletion();
    //             await _storageObjectRepository.RemoveAsync(existingStorageObject);
    //         }
    //
    //         var storageObject = result.Value[0]; // TODO: iterate over the list of results
    //         storageObject.BucketId = bucket.Id;
    //
    //         await _storageObjectRepository.AddAsync(storageObject);
    //         
    //         if(saveChanges)
    //             await _storageObjectRepository.SaveChangesAsync();
    //         
    //         return storageObject;
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "An exception occurred whilst uploading file (\'{FileName}\')",
    //             saveStorageObjectRequest.GdFilePath);
    //         return null;
    //     }
    // }

    private StorageObject ConvertSaveRequestToStorageObject(
        SaveStorageObjectRequest request,
        Guid temporaryFileKey
    )
    {
        var storageId = Guid.NewGuid(); // Create a new GUID so that we can use it to generate the GameDrivePath
        return new StorageObject()
        {
            Id = storageId,
            OwnerId = request.OwnerId,
            BucketId = request.BucketId,

            FileSizeBytes = 0,
            FileHash = request.FileHash,

            ClientRelativePath = request.GdFilePath,

            UploadedDate = DateTime.Now,
            CreatedDate = request.FileCreatedDate,
            LastModifiedDate = request.FileLastModifiedDate,

            GameDrivePath = Path.Combine("storage", $"{storageId.ToString().Replace("-", "")}.blob"),
            TemporaryFileKey = temporaryFileKey
        };
    }

    private async Task<StorageObject?> GetExistingStorageObject(string bucketId, string gdFilePath)
    {
        var existingStorageObject = (await _storageObjectRepository
                .FindAsync(x => x.BucketId == bucketId && x.ClientRelativePath == gdFilePath))
                .FirstOrDefault();

        return existingStorageObject;
    }
    
    private async Task<Bucket> GetOrCreateBucketAsync(Bucket newBucket)
    {
        var existingBucket = (await _bucketRepository
                .FindAsync(x => x.Id == newBucket.Id))
                .FirstOrDefault();

        if (existingBucket is not null) 
            return existingBucket;
        
        await _bucketRepository.AddAsync(newBucket);
        return newBucket;
    }
}