using GameDrive.Server.Domain.Models;
using GameDrive.Server.Services.Repositories;
using Microsoft.AspNetCore.WebUtilities;

namespace GameDrive.Server.Services.Storage;

public class StorageService
{
    private readonly ILogger<StorageService> _logger;
    private readonly IStorageProvider _storageProvider;
    private readonly StorageObjectRepository _storageObjectRepository;

    public StorageService(
        ILogger<StorageService> logger,
        IStorageProvider storageProvider,
        StorageObjectRepository storageObjectRepository
    )
    {
        _logger = logger;
        _storageProvider = storageProvider;
        _storageObjectRepository = storageObjectRepository;
    }

    public async Task<StorageObject?> UploadFileAsync(string clientPath, MultipartReader multipartReader, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await _storageProvider.SaveObjectAsync(new SaveStorageObjectRequest(
                OwnerId: 0, // TODO: extract this from JWT payload
                ClientPath: clientPath,
                MultipartReader: multipartReader
            ), cancellationToken);

            if (!result.Success)
                return null;

            var storageObject = result.StorageObject!;
            await _storageObjectRepository.AddAsync(storageObject);
            await _storageObjectRepository.SaveChangesAsync();
            return storageObject;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An exception occurred whilst uploading file (\'{ClientPath}\')", clientPath);
            return null;
        }
    }
}