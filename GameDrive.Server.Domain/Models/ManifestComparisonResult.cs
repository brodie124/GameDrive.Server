using System.Text.Json.Serialization;

namespace GameDrive.Server.Domain.Models;

public class ManifestFileReport
{
    [JsonPropertyName("crossReferenceId")]
    public Guid CrossReferenceId { get; set; }
    
    [JsonPropertyName("uploadState")]
    public FileUploadState UploadState { get; set; }
    
    [JsonPropertyName("diffState")]
    public  FileDiffState DiffState { get; set; }
    
    [JsonPropertyName("storageObjectId")]
    public Guid? StorageObjectId { get; set; }

    [JsonPropertyName("storageObjectHash")]
    public string? StorageObjectHash { get; set; }
    
    [JsonPropertyName("clientRelativePath")]
    public string? ClientRelativePath { get; set; }

    public ManifestFileReport()
    {
        
    }

    public ManifestFileReport(
        Guid crossReferenceId,
        FileUploadState uploadState, 
        FileDiffState diffState, 
        StorageObject? storageObject
    )
    {
        CrossReferenceId = crossReferenceId;
        UploadState = uploadState;
        DiffState = diffState;
        StorageObjectId = storageObject?.Id;
        StorageObjectHash = storageObject?.FileHash;
        ClientRelativePath = storageObject?.ClientRelativePath;
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
    Missing,
    Same,
    Conflict
}