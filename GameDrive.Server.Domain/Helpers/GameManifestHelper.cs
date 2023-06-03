using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameDrive.Server.Domain.Helpers;

public static class GameManifestHelper
{
    public static async Task<List<LudusaviGameProfile>?> ParseManifestAsync(string filePath)
    {
        await using var fileReader = File.OpenRead(filePath);
        var ludusaviGameList = await JsonSerializer.DeserializeAsync<List<LudusaviGameProfile>>(fileReader);
        return ludusaviGameList;
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Tag
    {
        Config = 1,
        Save = 2
    }

    public class LudusaviGameProfile
    {
        [JsonPropertyName("gdId")]
        public string Id { get; set; }
        
        [JsonPropertyName("name")] 
        public string Name { get; set; }

        [JsonPropertyName("files")] 
        public List<LudusaviGameFile> Files { get; set; } = new List<LudusaviGameFile>();

        [JsonPropertyName("launch")]
        public List<LudusaviGameLaunchConfiguration>? LaunchConfigurations { get; set; } =
            new List<LudusaviGameLaunchConfiguration>();

        [JsonPropertyName("registry")]
        public List<LudusaviGameRegistryItem>? RegistryItems { get; set; } = new List<LudusaviGameRegistryItem>();

        [JsonPropertyName("steam")] 
        public LudusaviSteamData? SteamData { get; set; }

        [JsonPropertyName("gog")] 
        public LudusaviGogData? GogData { get; set; }

        public class LudusaviGameFile
        {
            [JsonPropertyName("path")] 
            public string Path { get; set; }

            [JsonPropertyName("tags")] 
            public List<Tag> Tags { get; set; } = new List<Tag>();

            [JsonPropertyName("when")]
            public List<LudusaviFileConstraint> Constraints { get; set; } = new List<LudusaviFileConstraint>();

            public class LudusaviFileConstraint
            {
                [JsonPropertyName("os")] 
                public string? OperatingSystem { get; set; }

                [JsonPropertyName("store")] 
                public string? Store { get; set; }
            }
        }

        public class LudusaviGameLaunchConfiguration
        {
            [JsonPropertyName("path")] public string Path { get; set; }

            [JsonPropertyName("options")]
            public List<LudusaviGameLaunchConfigurationItem> Configurations { get; set; } =
                new List<LudusaviGameLaunchConfigurationItem>();


            public class LudusaviGameLaunchConfigurationItem
            {
                [JsonPropertyName("workingDir")] public string? WorkingDirectory { get; set; }

                [JsonPropertyName("arguments")] public string? Arguments { get; set; }

                [JsonPropertyName("when")]
                public List<LudusaviLaunchConstraint> Constraints { get; set; } = new List<LudusaviLaunchConstraint>();
            }

            public class LudusaviLaunchConstraint
            {
                [JsonPropertyName("bit")]
                public int? Architecture { get; set; }

                [JsonPropertyName("os")]
                public string? OperatingSystem { get; set; }

                [JsonPropertyName("store")] 
                public string? Store { get; set; }
            }
        }

        public class LudusaviGameRegistryItem
        {
            [JsonPropertyName("path")] public string Path { get; set; }

            [JsonPropertyName("tags")] public List<Tag> Tags { get; set; } = new List<Tag>();

            [JsonPropertyName("when")]
            public List<LudusaviRegistryConstraint> Constraints { get; set; } = new List<LudusaviRegistryConstraint>();

            public class LudusaviRegistryConstraint
            {
                [JsonPropertyName("store")] public string Store { get; set; }
            }
        }

        public class LudusaviSteamData
        {
            [JsonPropertyName("id")] public ulong Id { get; set; }
        }

        public class LudusaviGogData
        {
            [JsonPropertyName("id")] public ulong Id { get; set; }
        }
    }
}