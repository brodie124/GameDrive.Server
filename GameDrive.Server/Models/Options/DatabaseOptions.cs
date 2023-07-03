namespace GameDrive.Server.Models.Options;

public class DatabaseOptions
{
    public const string SectionName = "Database";
    public string MysqlConnectionString { get; set; } = default!;
    public string SqliteConnectionString { get; set; } = default!;
}