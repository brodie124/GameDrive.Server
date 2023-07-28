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
        await using var writeStream = new StreamWriter(filePath);
        await source.CopyToAsync(writeStream.BaseStream);
        await writeStream.FlushAsync();
        return new SaveFileResult(
            Key: key,
            Hash: string.Empty
        );
    }
    
    public Stream GetFile(Guid key)
    {
        var relativePath = MakeTemporaryPath(key);
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
        if (!File.Exists(filePath))
            throw new InvalidOperationException("Specified key cannot be found in temp storage");
        
        var readStream = new StreamReader(filePath);
        return readStream.BaseStream;
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