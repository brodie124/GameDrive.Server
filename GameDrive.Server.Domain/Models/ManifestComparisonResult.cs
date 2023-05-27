using System.Text.Json.Serialization;

namespace GameDrive.Server.Domain.Models;

public class ManifestFileReport
{
    [JsonPropertyName("uploadState")]
    public FileUploadState UploadState { get; set; }
    
    [JsonPropertyName("diffState")]
    public  FileDiffState DiffState { get; set; }

    public ManifestFileReport()
    {
        
    }

    public ManifestFileReport(FileUploadState uploadState, FileDiffState diffState)
    {
        UploadState = uploadState;
        DiffState = diffState;
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