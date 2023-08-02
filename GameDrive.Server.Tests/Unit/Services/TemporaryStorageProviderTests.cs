using System.Security.Cryptography;
using GameDrive.Server.Models.Options;
using GameDrive.Server.Services.Storage;
using GameDrive.Server.Utilities;
using Microsoft.Extensions.Options;

namespace GameDrive.Server.Tests.Unit.Services;

public class TemporaryStorageProviderTests
{
    private TemporaryStorageProvider _sut;
    public TemporaryStorageProviderTests()
    {
        var options = Options.Create(new TemporaryStorageOptions()
        {
            TemporaryStoragePath = "./storage/tests/"
        });
        _sut = new TemporaryStorageProvider(options);
    }

    [Fact]
    public async void SaveFileAsync_ReturnsCorrectHash()
    {
        var data = new byte[] { (byte) 'h', (byte) 'e', (byte) 'l', (byte) 'l', (byte) 'o',(byte) ' ',(byte) 'w',(byte) 'o', (byte) 'r', (byte) 'l', (byte) 'd' };
        var dataHash = HashHelper.Sha1String(data);
        var dataAltHash = IsolatedSha1(data);
        
        var stream = new MemoryStream();
        await stream.WriteAsync(data);
        stream.Position = 0;
        
        var result = await _sut.SaveFileAsync(stream);
        var path = _sut.GetFilePath(result.Key);
        var returnData = await File.ReadAllBytesAsync(path);

        Assert.Equal(data.Length, returnData.Length);
        Assert.Equal(dataHash, dataAltHash);
        Assert.Equal(dataHash, result.Hash);
    }

    private string IsolatedSha1(byte[] data)
    {
        using var hasher = HashAlgorithm.Create("SHA1")!;
        var hashBytes = hasher.ComputeHash(data);
        var hash = Convert.ToBase64String(hashBytes);
        return hash;
    }
}