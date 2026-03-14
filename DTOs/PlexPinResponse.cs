using System.Text.Json.Serialization;

namespace CheapNights.DTOs;

public class PlexPinResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; } = "";

    [JsonPropertyName("authToken")]
    public string? AuthToken { get; set; }
}
