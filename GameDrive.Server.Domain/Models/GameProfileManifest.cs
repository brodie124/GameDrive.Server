namespace GameDrive.Server.Domain.Models;

public class GameProfileManifest
{
    public string GameProfileId { get; set; }
    public IReadOnlyCollection<ManifestEntry> Entries { get; set; }
}

public class ManifestEntry
{
    /// <summary>
    /// A globally unique identifier used to keep track of which object we are referring to
    /// </summary>
    public Guid Guid { get; set; }
    
    /// <summary>
    /// Whether the file has been detected as deleted by the client since it was last synchronised
    /// </summary>
    public bool IsDeleted { get; set; }
    
    /// <summary>
    /// The file path relative to the game save directory
    /// </summary>
    public string RelativePath { get; set; }
    
    /// <summary>
    /// SHA256 hash of the files contents
    /// </summary>
    public string? FileHash { get; set; }
    
    /// <summary>
    /// The size of the file in bytes
    /// </summary>
    public long FileSize { get; set; }
    
    /// <summary>
    /// The previously known last modified date of the file, if available.
    /// This differs from LastModifiedDate in that the client has to have previously synchronised this file
    /// to have this value.
    /// </summary>
    public DateTime? ClientPreviousLastModifiedDate { get; set; }
    
    /// <summary>
    /// The last modified date retrieved from the files metadata
    /// </summary>
    public DateTime LastModifiedDate { get; set; }
    
    /// <summary>
    /// The created date retrieved from the files metadata
    /// </summary>
    public DateTime CreatedDate { get; set; }
}