using GameDrive.Server.Domain.Models;
using GameDrive.Server.Models.Options;
using Microsoft.Extensions.Options;

namespace GameDrive.Server.Services.Storage;

public class LocalCloudStorageProvider : ICloudStorageProvider
{
    private readonly TemporaryStorageOptions _temporaryStorageOptions;

    public LocalCloudStorageProvider(
        IOptions<TemporaryStorageOptions> temporaryStorageOptions)
    {
        _temporaryStorageOptions = temporaryStorageOptions.Value;
    }
    
    public async Task<SaveStorageObjectResult> SaveObjectAsync(SaveStorageObjectRequest saveRequest, CancellationToken cancellationToken = default)
    {
        // TODO: do this on application startup rather than every time a file is saved...
        if (!Directory.Exists("storage"))
            Directory.CreateDirectory("storage");
        
        var storageId = Guid.NewGuid(); // Create a new GUID so that we can use it to generate the GameDrivePath
        var storageObject = new StorageObject() {
            Id = storageId,
            OwnerId = saveRequest.OwnerId,
            BucketId = saveRequest.BucketId,
            
            FileSizeBytes = 0,
            FileHash = "",
            
            ClientRelativePath = saveRequest.GdFilePath,
            
            UploadedDate = DateTime.Now,
            CreatedDate = saveRequest.FileCreatedDate,
            LastModifiedDate = saveRequest.FileLastModifiedDate,

            GameDrivePath = Path.Combine("storage", $"{storageId.ToString().Replace("-", "")}.blob"),
        };

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), storageObject.GameDrivePath);
        var writeStream = new StreamWriter(filePath);
        await saveRequest.SourceStream.CopyToAsync(writeStream.BaseStream, cancellationToken);

        var fileSize = writeStream.BaseStream.Position;
        await writeStream.FlushAsync();
        writeStream.Close();
        
        storageObject.FileSizeBytes = fileSize;
        storageObject.FileHash = saveRequest.FileHash; // TODO: change this so that we compare the hash provided with a calculated hash on the uploaded file to verify integrity

        return new SaveStorageObjectResult(
            Success: true,
            StorageObject: storageObject
        );
    }

    public Task<DownloadStorageObjectResult> GenerateDownloadLinkAsync(StorageObject storageObject)
    {
        return Task.FromResult(new DownloadStorageObjectResult(
            Success: true,
            DownloadUrl: $"/Download/Local/{storageObject.Id}"
        ));
    }

    public async Task<DeleteStorageObjectResult> DeleteObjectAsync(StorageObject storageObject)
    {
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), storageObject.GameDrivePath);
        try
        {
            File.Delete(filePath);
        }
        catch (IOException ex)
        {
            Console.WriteLine("An error occurred whilst deleting the storage object!");
            Console.WriteLine(ex.Message);
            return new DeleteStorageObjectResult(
                Success: false
            );
        }

        return new DeleteStorageObjectResult(
            Success: true
        );
    }
}