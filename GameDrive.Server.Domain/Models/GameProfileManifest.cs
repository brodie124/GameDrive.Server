namespace GameDrive.Server.Domain.Models;

public class GameProfileManifest
{
    public string GameProfileId { get; set; }
    public IReadOnlyCollection<ManifestEntry> Entries { get; set; }
}

public class ManifestEntry
{
    public Guid Guid { get; set; }
    public bool IsDeleted { get; set; }
    public string RelativePath { get; set; }
    public string? FileHash { get; set; }
    public long FileSize { get; set; }
    public DateTime LastModifiedDate { get; set; }
    public DateTime CreatedDate { get; set; }
}