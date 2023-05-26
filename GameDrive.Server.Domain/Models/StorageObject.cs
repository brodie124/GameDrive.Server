using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameDrive.Server.Domain.Models;

// TODO: could de-duplicate files when they are uploaded, in case two people have the same file?
// TODO: include file hash information
[Table("storage_objects")]
public class StorageObject
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public int OwnerId { get; set; }
    public int GameProfileId { get; set; }

    
    public long FileSizeBytes { get; set; }
    public string FileExtension { get; set; } = default!;
    public string FileName { get; set; } = default!;
    public string FileDirectory { get; set; } = default!;
    public string FileHash { get; set; } = default!;
   
    public string GameDrivePath { get; set; } = default!;

    
    [ForeignKey(nameof(OwnerId))] 
    public User Owner { get; set; } = default!;
    
    [ForeignKey(nameof(GameProfileId))] 
    public GameProfile GameProfile { get; set; } = default!;
}