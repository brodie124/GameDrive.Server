using Microsoft.AspNetCore.WebUtilities;

namespace GameDrive.Server.Services.Storage;

public class StorageService
{
    private readonly IStorageProvider _storageProvider;

    public StorageService(IStorageProvider storageProvider)
    {
        _storageProvider = storageProvider;
    }

    public async Task<StorageObject?> UploadFileAsync(string clientPath, MultipartReader multipartReader, CancellationToken cancellationToken = default)
    {
        var result = await _storageProvider.SaveObjectAsync(new SaveStorageObjectRequest(
            ClientPath: clientPath,
            MultipartReader: multipartReader
        ), cancellationToken);

        return result.Success 
            ? result.StorageObject! 
            : null;
    }
}