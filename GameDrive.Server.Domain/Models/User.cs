using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameDrive.Server.Domain.Models;

[Table("users")]
public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Username { get; set; } = default!;
    
    [Required]
    public string PasswordHash { get; set; } = default!;
}