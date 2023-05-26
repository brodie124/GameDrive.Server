namespace GameDrive.Server.Domain.Helpers;

public class DirectoryPlaceholderHelper
{
    private static readonly IReadOnlyDictionary<string, string> DirectoryMappings = new Dictionary<string, string>()
    {
        { "<root>", ""},
        { "<game>", "" },
        { "<base>", "" },
        { "<home>", Environment.ExpandEnvironmentVariables("%USERPROFILE%") },
        { "<storeUserId>", "" },
        { "<osUserName>", Environment.UserName },
        { "<winAppData>", Environment.ExpandEnvironmentVariables("%APPDATA%")  },
        { "<winLocalAppData>", Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%")  },
        { "<winDocuments>", "" },
        { "<winPublic>", Environment.ExpandEnvironmentVariables("%PUBLIC%")  },
        { "<winProgramData>", Environment.ExpandEnvironmentVariables("%PROGRAMDATA%")  },
        { "<winDir>", Environment.ExpandEnvironmentVariables("%WINDIR%")  },
        { "<xdgData>", "" },
        { "<xdgConfig>", "" }
    };

    public static string ResolveGdPath(string gdPath)
    {
        gdPath = Environment.ExpandEnvironmentVariables(gdPath);
        gdPath = DirectoryMappings.Aggregate(gdPath, (current, mapping) => current.Replace(mapping.Key, mapping.Value));
        return gdPath;
    }
}