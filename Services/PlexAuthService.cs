using System.Net.Http.Headers;
using System.Xml.Linq;
using CheapHelpers.Caching;
using CheapNights.DTOs;
using CheapNights.Helpers;
using MimeMapping;

namespace CheapNights.Services;

public class PlexAuthService(IConfiguration configuration, IHttpClientFactory httpClientFactory) : IDisposable
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly string _clientId = configuration["Plex:ClientId"] ?? "CheapNights";
    private readonly string _productName = configuration["Plex:ProductName"] ?? "CheapNights";
    private readonly string _adminToken = configuration["Plex:AdminToken"] ?? throw new InvalidOperationException("Plex:AdminToken is not configured. Set it via user-secrets (dev) or environment variable (prod).");
    private readonly AbsoluteExpirationCache<string> _tokenCache = new("PlexTokens", TimeSpan.FromDays(30));
    private readonly SemaphoreSlim _ownerLock = new(1, 1);
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

        var response = await client.PostAsync($"{PlexConstants.PlexApiBase}/pins?strong=true", null);
        if (!response.IsSuccessStatusCode) return null;

        var json = await response.Content.ReadFromJsonAsync<PlexPinResponse>();
        return json is null ? null : (json.Id, json.Code);
    }

    /// <summary>
    /// Builds the Plex auth redirect URL for the given PIN code.
    /// </summary>
    public string GetAuthRedirectUrl(string pinCode, string forwardUrl)
    {
        return $"{PlexConstants.PlexAuthUrl}?clientID={_clientId}&code={pinCode}&forwardUrl={Uri.EscapeDataString(forwardUrl)}&context%5Bdevice%5D%5Bproduct%5D={_productName}";
    }

    /// <summary>
    /// Polls a PIN to check if the user has authenticated. Returns the auth token or null.
    /// </summary>
    public async Task<string?> CheckPinAsync(int pinId)
    {
        using var client = CreateClient();

        var response = await client.GetAsync($"{PlexConstants.PlexApiBase}/pins/{pinId}");
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

        var response = await client.GetAsync($"{PlexConstants.PlexApiBase}/user");
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
        client.DefaultRequestHeaders.Add(PlexConstants.Headers.Token, _adminToken);

        var response = await client.GetAsync(PlexConstants.PlexUsersApi);
        if (!response.IsSuccessStatusCode) return false;

        var xml = await response.Content.ReadAsStringAsync();
        var doc = XDocument.Parse(xml);
        var userIdStr = plexUserId.ToString();

        return doc.Descendants("User").Any(u => u.Attribute("id")?.Value == userIdStr);
    }

    private async Task<int?> GetOwnerIdAsync()
    {
        if (_cachedOwnerId.HasValue) return _cachedOwnerId;

        await _ownerLock.WaitAsync();
        try
        {
            if (_cachedOwnerId.HasValue) return _cachedOwnerId;

            using var client = CreateClient(_adminToken);
            var response = await client.GetAsync($"{PlexConstants.PlexApiBase}/user");
            if (!response.IsSuccessStatusCode) return null;

            var owner = await response.Content.ReadFromJsonAsync<PlexUserInfo>();
            _cachedOwnerId = owner?.Id;
            return _cachedOwnerId;
        }
        finally
        {
            _ownerLock.Release();
        }
    }

    private HttpClient CreateClient(string? token = null)
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(KnownMimeTypes.Json));
        client.DefaultRequestHeaders.Add(PlexConstants.Headers.ClientIdentifier, _clientId);
        client.DefaultRequestHeaders.Add(PlexConstants.Headers.Product, _productName);
        if (token is not null)
            client.DefaultRequestHeaders.Add(PlexConstants.Headers.Token, token);
        return client;
    }

    public void Dispose()
    {
        _tokenCache.Dispose();
        _ownerLock.Dispose();
        GC.SuppressFinalize(this);
    }
}
