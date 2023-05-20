namespace GameDrive.Server.OptionsModels;

public class DatabaseOptions
{
    public const string SectionName = "Database";
    public string ConnectionString { get; set; } = default!;
}