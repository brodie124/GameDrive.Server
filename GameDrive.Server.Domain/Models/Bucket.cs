using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameDrive.Server.Domain.Models;

[Table("buckets")]
public class Bucket
{
    [Key]
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
}