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
    public CsvString SearchableDirectories { get; set; } = new CsvString(";");
    public CsvString IncludePatterns { get; set; } = new CsvString(";");
    public CsvString ExcludePatterns { get; set; } = new CsvString(";");
}