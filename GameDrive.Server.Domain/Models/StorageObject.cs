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
    public string ClientRelativePath { get; set; } = default!;
    public string FileHash { get; set; } = default!;
    
    public DateTime UploadedDate { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime LastModifiedDate { get; set; }
   
    public string? StoragePath { get; set; } = default!;

    public Guid? TemporaryFileKey { get; set; } = null;

    [NotMapped]
    public bool RequiresReplication => TemporaryFileKey is not null;
    
    public DateTime? ReplicationDate { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedDate { get; set; }

    [ForeignKey(nameof(OwnerId))] 
    public User Owner { get; set; } = default!;
    
    [ForeignKey(nameof(BucketId))] 
    public Bucket Bucket { get; set; } = default!;

    public void MarkForDeletion()
    {
        IsDeleted = true;
        DeletedDate = DateTime.Now;
    }
}