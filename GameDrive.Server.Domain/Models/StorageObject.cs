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
    public string BucketId { get; set; }
    
    public long FileSizeBytes { get; set; }
    public string FileNameWithExtension { get; set; } = default!;
    public string FileHash { get; set; } = default!;
    
    public DateTime UploadedDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }
   
    public string GameDrivePath { get; set; } = default!;

    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedDate { get; set; }

    [ForeignKey(nameof(OwnerId))] 
    public User Owner { get; set; } = default!;
    
    [ForeignKey(nameof(BucketId))] 
    public Bucket Bucket { get; set; } = default!;
}