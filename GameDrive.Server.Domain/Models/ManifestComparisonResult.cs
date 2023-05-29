using System.Text.Json.Serialization;

namespace GameDrive.Server.Domain.Models;

public class ManifestFileReport
{
    [JsonPropertyName("uploadState")]
    public FileUploadState UploadState { get; set; }
    
    [JsonPropertyName("diffState")]
    public  FileDiffState DiffState { get; set; }
    
    [JsonPropertyName("storageObjectId")]
    public Guid? StorageObjectId { get; set; }

    [JsonPropertyName("storageObjectHash")]
    public string? StorageObjectHash { get; set; }

    public ManifestFileReport()
    {
        
    }

    public ManifestFileReport(
        FileUploadState uploadState, 
        FileDiffState diffState, 
        StorageObject? storageObject
    )
    {
        UploadState = uploadState;
        DiffState = diffState;
        StorageObjectId = storageObject?.Id;
        StorageObjectHash = storageObject?.FileHash;
    }
}

public enum FileUploadState
{
    UploadRequested,
    DownloadAdvised,
    Ignore,
    Conflict
}

public enum FileDiffState
{
    New,
    Newer,
    Older,
    Removed,
    Same,
    Conflict
}