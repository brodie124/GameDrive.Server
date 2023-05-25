namespace GameDrive.Server.Models.Options;

public class DatabaseOptions
{
    public const string SectionName = "Database";
    public string ConnectionString { get; set; } = default!;
}