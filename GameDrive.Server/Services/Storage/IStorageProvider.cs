using System.Net.Http.Headers;
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
    string ClientPath,
    MultipartReader MultipartReader
);

public class LocalStorageProvider : IStorageProvider
{
    public async Task<SaveStorageObjectResult> SaveObjectAsync(SaveStorageObjectRequest saveRequest, CancellationToken cancellationToken = default)
    {
        // TODO: do this on application startup rather than every time a file is saved...
        if (!Directory.Exists("storage"))
            Directory.CreateDirectory("storage");
        
        var storageId = Guid.NewGuid();
        var storageObject = new StorageObject(
            ClientPath: saveRequest.ClientPath,
            GameDrivePath: Path.Combine("storage", $"{storageId.ToString().Replace("-", "")}.blob"),
            Id: storageId
        );

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), storageObject.GameDrivePath);
        var writeStream = new StreamWriter(filePath);
        
        var section = await saveRequest.MultipartReader.ReadNextSectionAsync(cancellationToken);
        while (section is not null)
        {
            if (!ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition))
                continue;
            
            if (contentDisposition.DispositionType != "form-data" || string.IsNullOrEmpty(contentDisposition.FileName))
            {
                continue;
            }

            await section.Body.CopyToAsync(writeStream.BaseStream, cancellationToken);
            section = await saveRequest.MultipartReader.ReadNextSectionAsync(cancellationToken);
        }
        
        await writeStream.FlushAsync();
        writeStream.Close();

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