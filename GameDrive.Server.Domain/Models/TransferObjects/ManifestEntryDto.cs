using System.Text.Json.Serialization;

namespace GameDrive.Server.Domain.Models.TransferObjects;

public class ManifestEntryDto
{
    [JsonPropertyName("guid")]
    public Guid Guid { get; set; }
    
    [JsonPropertyName("isDeleted")]
    public bool IsDeleted { get; set; }
    
    [JsonPropertyName("relativePath")]
    public string RelativePath { get; set; }
    
    [JsonPropertyName("fileHash")]
    public string? FileHash { get; set; }
    
    [JsonPropertyName("fileSize")]
    public long FileSize { get; set; }
    
    [JsonPropertyName("lastModifiedDate")]
    public DateTime LastModifiedDate { get; set; }
    
    [JsonPropertyName("createdDate")]
    public DateTime CreatedDate { get; set; }
}