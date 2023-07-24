namespace GameDrive.Server.Models.Options;

public class TemporaryStorageOptions
{
    public const string SectionName = "TemporaryStorage";
    public string TemporaryStoragePath { get; set; } = Path.Join(".", "temp");
}