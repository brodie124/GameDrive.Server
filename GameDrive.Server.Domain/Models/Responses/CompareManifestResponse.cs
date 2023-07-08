using System.Text.Json.Serialization;

namespace GameDrive.Server.Domain.Models.Responses;

public class CompareManifestResponse
{
    public List<CompareManifestResponseEntry> Entries { get; set; }

    public CompareManifestResponse(List<CompareManifestResponseEntry> entries)
    {
        Entries = entries;
    }
}

public class CompareManifestResponseEntry
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

    public CompareManifestResponseEntry()
    {
        
    }

    public CompareManifestResponseEntry(
        Guid crossReferenceId,
        FileUploadState uploadState,
        FileDiffState diffState
    )
    {
        CrossReferenceId = crossReferenceId;
        UploadState = uploadState;
        DiffState = diffState;
    }

    public CompareManifestResponseEntry WithStorageObject(StorageObject? storageObject)
    {
        StorageObjectId = storageObject?.Id;
        StorageObjectHash = storageObject?.FileHash;
        ClientRelativePath = storageObject?.ClientRelativePath;
        return this;
    }

    public CompareManifestResponseEntry WithEntry(ManifestEntry? manifestEntry)
    {
        ClientRelativePath = manifestEntry?.RelativePath;
        return this;
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