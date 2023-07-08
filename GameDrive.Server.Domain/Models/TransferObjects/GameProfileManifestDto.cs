using System.Text.Json.Serialization;

namespace GameDrive.Server.Domain.Models.TransferObjects;

public class GameProfileManifestDto
{
    [JsonPropertyName("gameProfileId")]
    public string GameProfileId { get; set; }
    
    [JsonPropertyName("entries")]
    public IReadOnlyCollection<ManifestEntryDto> Entries { get; set; }
}