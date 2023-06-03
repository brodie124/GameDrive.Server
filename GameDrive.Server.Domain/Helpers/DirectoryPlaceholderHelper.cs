namespace GameDrive.Server.Domain.Helpers;

public class DirectoryPlaceholderHelper
{
    private static readonly IReadOnlyDictionary<string, string?> DirectoryMappings = new Dictionary<string, string?>()
    {
        { "<root>", Path.Join(Environment.ExpandEnvironmentVariables("%PROGRAMFILES(X86)%"), "**") },
        { "<game>", "*" },
        { "<base>", "<root>/<game>" },
        { "<home>", Environment.ExpandEnvironmentVariables("%USERPROFILE%") },
        { "<storeUserId>", "*" },
        { "<osUserName>", Environment.UserName },
        { "<winAppData>", Environment.ExpandEnvironmentVariables("%APPDATA%")  },
        { "<winLocalAppData>", Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%")  },
        { "<winDocuments>", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) },
        { "<winPublic>", Environment.ExpandEnvironmentVariables("%PUBLIC%")  },
        { "<winProgramData>", Environment.ExpandEnvironmentVariables("%PROGRAMDATA%")  },
        { "<winDir>", null  },
        { "<xdgData>", null },
        { "<xdgConfig>", null }
    };

    public static string? ResolveGdPath(string gdPath)
    {
        // gdPath = Environment.ExpandEnvironmentVariables(gdPath);
        var isResolving = true;
        while (isResolving)
        {
            isResolving = false;
            foreach (var (key, value) in DirectoryMappings)
            {
                var containsKey = gdPath.Contains(key);
                isResolving = isResolving || containsKey;
                
                if (value is null && containsKey)
                    return null;

                if (value is null)
                    continue;

                gdPath = gdPath.Replace(key, value);
            }
        }

        return gdPath;
    }
}