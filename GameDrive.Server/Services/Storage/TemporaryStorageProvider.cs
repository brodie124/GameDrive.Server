using System.Security.Cryptography;
using GameDrive.Server.Models.Options;
using Microsoft.Extensions.Options;

namespace GameDrive.Server.Services.Storage;

public class TemporaryStorageProvider
{
    private readonly TemporaryStorageOptions _temporaryStorageOptions;

    public TemporaryStorageProvider(
        IOptions<TemporaryStorageOptions> temporaryStorageOptions
    )
    {
        _temporaryStorageOptions = temporaryStorageOptions.Value;
    }

    public async Task<SaveFileResult> SaveFileAsync(Stream source)
    {
        // TODO: do this on application startup rather than every time a file is saved...
        if (!Directory.Exists(_temporaryStorageOptions.TemporaryStoragePath))
            Directory.CreateDirectory(_temporaryStorageOptions.TemporaryStoragePath);
        
        var key = Guid.NewGuid();
        var relativePath = MakeTemporaryPath(key);
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
        await using var writeStream = File.OpenWrite(filePath);

        using var sha1 = SHA1.Create();
        sha1.Initialize();

        var buffer = new byte[256];
        while (true)
        {
            var bytesRead = await source.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead <= 0)
            {
                sha1.TransformFinalBlock(buffer, 0, 0);
                break;
            }
            
            sha1.TransformBlock(buffer, 0, bytesRead, null, 0);
            await writeStream.WriteAsync(buffer, 0, bytesRead);
        }
        
        await writeStream.FlushAsync();
        
        ArgumentNullException.ThrowIfNull(sha1.Hash);
        var fileHash = Convert.ToBase64String(sha1.Hash);
        
        return new SaveFileResult(
            Key: key,
            Hash: fileHash
        );
    }
    
    public Stream GetFileStream(Guid key)
    {
        var relativePath = MakeTemporaryPath(key);
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
        if (!File.Exists(filePath))
            throw new InvalidOperationException("Specified key cannot be found in temp storage");
        
        var readStream = new StreamReader(filePath);
        return readStream.BaseStream;
    }
    
    public string GetFilePath(Guid key)
    {
        var relativePath = MakeTemporaryPath(key);
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
        if (!File.Exists(filePath))
            throw new InvalidOperationException("Specified key cannot be found in temp storage");

        return filePath;
    }


    public Task<bool> HasFileAsync(Guid key)
    {
        var path = MakeTemporaryPath(key);
        return Task.FromResult(File.Exists(path));
    }

    private string MakeTemporaryPath(Guid key)
    {
        return Path.Combine(_temporaryStorageOptions.TemporaryStoragePath, $"{key.ToString()}.blob");
    } 

    public record SaveFileResult(
        Guid Key,
        string Hash
    );
}