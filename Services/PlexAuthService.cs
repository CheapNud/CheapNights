using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CheapNights.Services;

public class PlexAuthService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
{
    private const string PlexApiBase = "https://plex.tv/api/v2";
    private const string PlexAuthUrl = "https://app.plex.tv/auth#";

    private string ClientId => configuration["Plex:ClientId"] ?? "CheapNights";
    private string ProductName => configuration["Plex:ProductName"] ?? "CheapNights";
    private string AdminToken => configuration["Plex:AdminToken"] ?? "";

    /// <summary>
    /// Creates a PIN on Plex and returns (pinId, pinCode) for the auth flow.
    /// </summary>
    public async Task<(int PinId, string PinCode)?> CreatePinAsync()
    {
        using var client = CreateClient();

        var response = await client.PostAsync($"{PlexApiBase}/pins?strong=true", null);
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadFromJsonAsync<PlexPinResponse>();
        return json is null ? null : (json.Id, json.Code);
    }

    /// <summary>
    /// Builds the Plex auth redirect URL for the given PIN code.
    /// </summary>
    public string GetAuthRedirectUrl(string pinCode, string forwardUrl)
    {
        return $"{PlexAuthUrl}?clientID={ClientId}&code={pinCode}&forwardUrl={Uri.EscapeDataString(forwardUrl)}&context%5Bdevice%5D%5Bproduct%5D={ProductName}";
    }

    /// <summary>
    /// Polls a PIN to check if the user has authenticated. Returns the auth token or null.
    /// </summary>
    public async Task<string?> CheckPinAsync(int pinId)
    {
        using var client = CreateClient();

        var response = await client.GetAsync($"{PlexApiBase}/pins/{pinId}");
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadFromJsonAsync<PlexPinResponse>();
        return string.IsNullOrEmpty(json?.AuthToken) ? null : json.AuthToken;
    }

    /// <summary>
    /// Gets the Plex user profile from an auth token.
    /// </summary>
    public async Task<PlexUserInfo?> GetUserAsync(string authToken)
    {
        using var client = CreateClient(authToken);

        var response = await client.GetAsync($"{PlexApiBase}/user");
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<PlexUserInfo>();
    }

    /// <summary>
    /// Checks if a Plex user has access to the admin's server by comparing shared user IDs.
    /// The admin (server owner) always has access.
    /// </summary>
    public async Task<bool> HasServerAccessAsync(string authToken, int plexUserId)
    {
        // Check if this user is the server owner
        using var ownerClient = CreateClient(AdminToken);
        var ownerResponse = await ownerClient.GetAsync($"{PlexApiBase}/user");
        if (ownerResponse.IsSuccessStatusCode)
        {
            var owner = await ownerResponse.Content.ReadFromJsonAsync<PlexUserInfo>();
            if (owner?.Id == plexUserId) return true;
        }

        // Check shared users (friends) via the XML API
        using var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Plex-Token", AdminToken);

        var response = await client.GetAsync("https://plex.tv/api/users");
        if (!response.IsSuccessStatusCode) return false;

        var xml = await response.Content.ReadAsStringAsync();
        return xml.Contains($"id=\"{plexUserId}\"");
    }

    private HttpClient CreateClient(string? token = null)
    {
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("X-Plex-Client-Identifier", ClientId);
        client.DefaultRequestHeaders.Add("X-Plex-Product", ProductName);
        if (token is not null)
            client.DefaultRequestHeaders.Add("X-Plex-Token", token);
        return client;
    }
}

public class PlexPinResponse
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("code")]
    public string Code { get; set; } = "";

    [JsonPropertyName("authToken")]
    public string? AuthToken { get; set; }
}

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
