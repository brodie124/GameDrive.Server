namespace GameDrive.Server.Domain.Models;

public class FileManifest
{
    public string BucketId { get; set; }
    public IReadOnlyCollection<ManifestEntry> Entries { get; set; }
}

public class ManifestEntry
{
    public Guid Guid { get; set; }
    
    public string FullClientPath { get; set; }
    public string FileName { get; set; }
    public string FileHash { get; set; }
    public long FileSize { get; set; }
    public DateTime LastModifiedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}