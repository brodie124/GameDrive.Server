namespace GameDrive.Server.Domain;

public static class GdFileExtensionBlockList
{
    public static readonly string[] BlockedExtensions = new[]
    {
        ".png",
        ".jpg",
        ".jpeg",
        ".tiff",
        ".bmp",
        ".rar",
        ".zip",
        ".vdf",
        ".ini",
        ".bak",
        ".log",
        ".tmp",
    };

    public static bool IsAllowed(string path)
    {
        var lowerCasePath = path.ToLower();
        return !BlockedExtensions.Any(x => lowerCasePath.EndsWith(x));
    }
}