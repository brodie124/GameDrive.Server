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
    public string ClientPath { get; set; } = default!;
    public string GameDrivePath { get; set; } = default!;
    
}