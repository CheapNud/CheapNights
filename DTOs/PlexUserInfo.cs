using System.Text.Json.Serialization;

namespace CheapNights.DTOs;

public class PlexUserInfo
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; } = "";

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("thumb")]
    public string? Thumb { get; set; }
}
