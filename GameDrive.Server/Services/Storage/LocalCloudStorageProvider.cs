using CSharpFunctionalExtensions;
using GameDrive.Server.Domain.Models;
using GameDrive.Server.Models.Options;
using Microsoft.Extensions.Options;

namespace GameDrive.Server.Services.Storage;

public class LocalCloudStorageProvider : ICloudStorageProvider
{
    private readonly TemporaryStorageProvider _temporaryStorageProvider;

    public LocalCloudStorageProvider(
        TemporaryStorageProvider temporaryStorageProvider
    )
    {
        _temporaryStorageProvider = temporaryStorageProvider;
    }

    public async Task<IReadOnlyList<SaveStorageObjectResult>> SaveObjectsAsync(
        IEnumerable<StorageObject> storageObjects,
        CancellationToken cancellationToken = default
    )
    {
        // TODO: do this on application startup rather than every time a file is saved...
        if (!Directory.Exists("storage"))
            Directory.CreateDirectory("storage");

        var results = new List<SaveStorageObjectResult>();
        foreach (var storageObject in storageObjects.ToList())
        {
            if (storageObject.TemporaryFileKey is null)
            {
                results.Add(new SaveStorageObjectResult(
                    StorageObjectId: storageObject.Id,
                    Success: false,
                    ErrorMessage: "Upload called for object that has already been replicated"
                ));
                continue;
            }

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), storageObject.StoragePath);
            await using var writeStream = new StreamWriter(filePath);
            await using var temporaryFileStream =
                _temporaryStorageProvider.GetFileStream((Guid)storageObject.TemporaryFileKey);

            await temporaryFileStream.CopyToAsync(writeStream.BaseStream, cancellationToken);
            await writeStream.FlushAsync();
            results.Add(new SaveStorageObjectResult(
                StorageObjectId: storageObject.Id,
                Success: true
            ));
        }

        return results;
    }

    public Task<Result<string>> GenerateDownloadLinkAsync(StorageObject storageObject)
    {
        return Task.FromResult(
            Result.Success($"/Download/Local/{storageObject.Id}")
        );
    }

    public async Task<Result> DeleteObjectAsync(StorageObject storageObject)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), storageObject.StoragePath);
        try
        {
            File.Delete(filePath);
            return Result.Success();
        }
        catch (IOException ex)
        {
            Console.WriteLine("An error occurred whilst deleting the storage object!");
            Console.WriteLine(ex.Message);
            return Result.Failure("An IO-related error occurred whilst deleting the storage object.");
        }
    }
}