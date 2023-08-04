namespace GameDrive.Server.Models.Options;

public class AwsOptions
{
    public const string SectionName = "Aws";
    public string? AccessKey { get; set; }
    public string? SecretAccessKey { get; set; }
}