using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace CheapNights.Services;

public class PlexAuthService
{
    private const string PlexApiBase = "https://plex.tv/api/v2";
    private const string PlexAuthUrl = "https://app.plex.tv/auth#";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _clientId;
    private readonly string _productName;
    private readonly string _adminToken;

    public PlexAuthService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _clientId = configuration["Plex:ClientId"] ?? "CheapNights";
        _productName = configuration["Plex:ProductName"] ?? "CheapNights";
        _adminToken = configuration["Plex:AdminToken"]
            ?? throw new InvalidOperationException("Plex:AdminToken is not configured. Set it via user-secrets (dev) or environment variable (prod).");
    }

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
        return $"{PlexAuthUrl}?clientID={_clientId}&code={pinCode}&forwardUrl={Uri.EscapeDataString(forwardUrl)}&context%5Bdevice%5D%5Bproduct%5D={_productName}";
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
        using var ownerClient = CreateClient(_adminToken);
        var ownerResponse = await ownerClient.GetAsync($"{PlexApiBase}/user");
        if (ownerResponse.IsSuccessStatusCode)
        {
            var owner = await ownerResponse.Content.ReadFromJsonAsync<PlexUserInfo>();
            if (owner?.Id == plexUserId) return true;
        }

        // Check shared users (friends) via the XML API
        using var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Plex-Token", _adminToken);

        var response = await client.GetAsync("https://plex.tv/api/users");
        if (!response.IsSuccessStatusCode) return false;

        var xml = await response.Content.ReadAsStringAsync();
        var doc = XDocument.Parse(xml);
        var userIdStr = plexUserId.ToString();

        return doc.Descendants("User").Any(u => u.Attribute("id")?.Value == userIdStr);
    }

    private HttpClient CreateClient(string? token = null)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Add("X-Plex-Client-Identifier", _clientId);
        client.DefaultRequestHeaders.Add("X-Plex-Product", _productName);
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
