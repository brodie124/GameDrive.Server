using CSharpFunctionalExtensions;
using GameDrive.Server.Domain.Models;
using GameDrive.Server.Models.Options;
using Microsoft.Extensions.Options;

namespace GameDrive.Server.Services.Storage;

public class LocalCloudStorageProvider : ICloudStorageProvider
{
    public async Task<Result<IReadOnlyList<StorageObject>>> SaveObjectsAsync(
        IEnumerable<SaveStorageObjectRequest> saveRequests, 
        CancellationToken cancellationToken = default
    )
    {
        // TODO: do this on application startup rather than every time a file is saved...
        if (!Directory.Exists("storage"))
            Directory.CreateDirectory("storage");

        var storageObjects = new List<StorageObject>();
        foreach (var request in saveRequests)
        {

            var storageId = Guid.NewGuid(); // Create a new GUID so that we can use it to generate the GameDrivePath
            var storageObject = new StorageObject()
            {
                Id = storageId,
                OwnerId = request.OwnerId,
                BucketId = request.BucketId,

                FileSizeBytes = 0,
                FileHash = "",

                ClientRelativePath = request.GdFilePath,

                UploadedDate = DateTime.Now,
                CreatedDate = request.FileCreatedDate,
                LastModifiedDate = request.FileLastModifiedDate,

                GameDrivePath = Path.Combine("storage", $"{storageId.ToString().Replace("-", "")}.blob"),
            };

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), storageObject.GameDrivePath);
            var writeStream = new StreamWriter(filePath);
            await request.SourceStream.CopyToAsync(writeStream.BaseStream, cancellationToken);

            var fileSize = writeStream.BaseStream.Position;
            await writeStream.FlushAsync();
            writeStream.Close();

            storageObject.FileSizeBytes = fileSize;
            storageObject.FileHash =
                request.FileHash; // TODO: change this so that we compare the hash provided with a calculated hash on the uploaded file to verify integrity

            storageObjects.Add(storageObject);
        }

        return storageObjects;
    }

    public Task<Result<string>> GenerateDownloadLinkAsync(StorageObject storageObject)
    {
        return Task.FromResult(
            Result.Success($"/Download/Local/{storageObject.Id}")
        );
    }

    public async Task<Result> DeleteObjectAsync(StorageObject storageObject)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), storageObject.GameDrivePath);
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