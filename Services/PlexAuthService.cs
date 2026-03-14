using System.Net.Http.Headers;
using System.Xml.Linq;
using CheapHelpers.Caching;
using CheapNights.DTOs;

namespace CheapNights.Services;

public class PlexAuthService(IConfiguration configuration, IHttpClientFactory httpClientFactory) : IDisposable
{
    private const string PlexApiBase = "https://plex.tv/api/v2";
    private const string PlexAuthUrl = "https://app.plex.tv/auth#";

    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly string _clientId = configuration["Plex:ClientId"] ?? "CheapNights";
    private readonly string _productName = configuration["Plex:ProductName"] ?? "CheapNights";
    private readonly string _adminToken = configuration["Plex:AdminToken"]
            ?? throw new InvalidOperationException("Plex:AdminToken is not configured. Set it via user-secrets (dev) or environment variable (prod).");
    private readonly AbsoluteExpirationCache<string> _tokenCache = new("PlexTokens", TimeSpan.FromDays(30));
    private int? _cachedOwnerId;

    /// <summary>
    /// Stores a Plex auth token server-side, keyed by PlexUserId.
    /// </summary>
    public void StoreToken(int plexUserId, string authToken)
    {
        _tokenCache.Set($"{plexUserId}", authToken);
    }

    /// <summary>
    /// Retrieves a stored Plex auth token for the given user.
    /// </summary>
    public string? GetStoredToken(int plexUserId)
    {
        return _tokenCache.TryGet($"{plexUserId}", out var token) ? token : null;
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
    /// Checks if a Plex user has access to the admin's server.
    /// The admin (server owner) always has access. Owner ID is cached after first lookup.
    /// </summary>
    public async Task<bool> HasServerAccessAsync(int plexUserId)
    {
        var ownerId = await GetOwnerIdAsync();
        if (ownerId == plexUserId) return true;

        using var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Plex-Token", _adminToken);

        var response = await client.GetAsync("https://plex.tv/api/users");
        if (!response.IsSuccessStatusCode) return false;

        var xml = await response.Content.ReadAsStringAsync();
        var doc = XDocument.Parse(xml);
        var userIdStr = plexUserId.ToString();

        return doc.Descendants("User").Any(u => u.Attribute("id")?.Value == userIdStr);
    }

    private async Task<int?> GetOwnerIdAsync()
    {
        if (_cachedOwnerId.HasValue) return _cachedOwnerId;

        using var client = CreateClient(_adminToken);
        var response = await client.GetAsync($"{PlexApiBase}/user");
        if (!response.IsSuccessStatusCode) return null;

        var owner = await response.Content.ReadFromJsonAsync<PlexUserInfo>();
        _cachedOwnerId = owner?.Id;
        return _cachedOwnerId;
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

    public void Dispose()
    {
        _tokenCache.Dispose();
        GC.SuppressFinalize(this);
    }
}
