using System.Text.Json.Serialization;
using GameDrive.Server.Domain.Models.TransferObjects;

namespace GameDrive.Server.Domain.Models.Requests;

public class CompareManifestRequest
{
    [JsonPropertyName("manifest")]
    public GameProfileManifestDto Manifest { get; set; } = default!;
}