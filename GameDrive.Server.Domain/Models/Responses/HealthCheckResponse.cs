using System.Text.Json.Serialization;

namespace GameDrive.Server.Domain.Models.Responses;

public class HealthCheckResponse
{
    [JsonPropertyName("isHealthy")]
    public bool IsHealthy { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    public HealthCheckResponse(bool isHealthy, string? version)
    {
        IsHealthy = isHealthy;
        Version = version ?? "0.0.0.0";
    }
}