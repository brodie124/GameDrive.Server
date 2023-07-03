using Gameloop.Vdf;
using Microsoft.CSharp.RuntimeBinder;

namespace GameDrive.Server.Domain.Helpers;

public class SteamLibraryHelper
{

    public static async Task<IReadOnlyList<SteamLibraryFolder>> GetSteamLibraryPaths()
    {
        var steamInstallPath = FindSteamInstallLocation();
        var vdfPath = Path.Combine(steamInstallPath, "steamapps", "libraryfolders.vdf");
        var vdfContents = await File.ReadAllTextAsync(vdfPath);
        dynamic libraryFolders = VdfConvert.Deserialize(vdfContents);

        try
        {
            var steamLibraries = new List<SteamLibraryFolder>();
            foreach (var folder in libraryFolders.Value)
            {
                var index = folder.Key;
                var library = folder.Value;
                var libraryPath = library.path;

                var libraryApps = library.apps;
                var installedAppIds = new List<string>();
                foreach (var installData in libraryApps)
                {
                    installedAppIds.Add(installData.Key.ToString());
                }

                steamLibraries.Add(new SteamLibraryFolder(
                    Path: Path.Join(libraryPath.ToString() ?? "", "steamapps").ToString(),
                    InstalledAppIds: installedAppIds
                ));
            }

            return steamLibraries;
        }
        catch (RuntimeBinderException)
        {
            return Array.Empty<SteamLibraryFolder>();
        }
    }
    

    public static string? FindSteamInstallLocation()
    {
        var path = Environment.ExpandEnvironmentVariables(Path.Join("%PROGRAMFILES(X86)%", "Steam"));
        return Directory.Exists(path)
            ? path
            : null;
    }
}

public record SteamLibraryFolder(
    string Path, 
    IReadOnlyList<string> InstalledAppIds
);