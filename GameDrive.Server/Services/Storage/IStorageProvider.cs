using Microsoft.Net.Http.Headers;
using GameDrive.Server.Domain.Models;
using GameDrive.Server.Utilities;
using Microsoft.AspNetCore.WebUtilities;

namespace GameDrive.Server.Services.Storage;

public interface IStorageProvider
{
    Task<SaveStorageObjectResult> SaveObjectAsync(SaveStorageObjectRequest saveRequest, CancellationToken cancellationToken = default);
    Task<DownloadStorageObjectResult> GenerateDownloadLinkAsync(StorageObject storageObject);
    Task<DeleteStorageObjectResult> DeleteObjectAsync(StorageObject storageObject);
}

public record SaveStorageObjectResult(
    bool Success,
    StorageObject? StorageObject
);

public record DeleteStorageObjectResult(
    bool Success
);

public record DownloadStorageObjectResult(
    bool Success,
    string DownloadUrl
);

public record SaveStorageObjectRequest(
    int OwnerId,
    int GameProfileId,
    
    string FileName,
    string FileExtension,
    string FileHash,
    
    MultipartReader MultipartReader
);

public class LocalStorageProvider : IStorageProvider
{
    public async Task<SaveStorageObjectResult> SaveObjectAsync(SaveStorageObjectRequest saveRequest, CancellationToken cancellationToken = default)
    {
        // TODO: do this on application startup rather than every time a file is saved...
        if (!Directory.Exists("storage"))
            Directory.CreateDirectory("storage");
        
        var storageId = Guid.NewGuid(); // Create a new GUID so that we can use it to generate the GameDrivePath
        var storageObject = new StorageObject() {
            Id = storageId,
            OwnerId = saveRequest.OwnerId,
            GameProfileId = saveRequest.GameProfileId,
            
            FileSizeBytes = 0,
            FileHash = "",
            
            FileName = saveRequest.FileName,
            FileExtension = saveRequest.FileExtension,

            GameDrivePath = Path.Combine("storage", $"{storageId.ToString().Replace("-", "")}.blob"),
        };

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), storageObject.GameDrivePath);
        var writeStream = new StreamWriter(filePath);
        
        var section = await saveRequest.MultipartReader.ReadNextSectionAsync(cancellationToken);
        while (section is not null)
        {
            if (!ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition))
                continue;

            if (!MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                continue;
            

            await section.Body.CopyToAsync(writeStream.BaseStream, cancellationToken);
            section = await saveRequest.MultipartReader.ReadNextSectionAsync(cancellationToken);
        }
        
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

    public async Task<DownloadStorageObjectResult> GenerateDownloadLinkAsync(StorageObject storageObject)
    {
        return new DownloadStorageObjectResult(
            Success: true,
            DownloadUrl: string.Empty
        );
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