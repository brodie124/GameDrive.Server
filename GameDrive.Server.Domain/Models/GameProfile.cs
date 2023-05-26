using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameDrive.Server.Domain.Models;

[Table("game_profiles")]
public class GameProfile
{
    [Key]
    public int Id { get; set; }
    public int Version { get; set; }
    public string Name { get; set; } = default!;
    public ICollection<string> SearchableDirectories = new List<string>();
    public ICollection<string> IncludePatterns = new List<string>();
    public ICollection<string> ExcludePatterns = new List<string>();
}