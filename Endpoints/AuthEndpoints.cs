using System.Security.Claims;
using CheapNights.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CheapNights.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        // Step 1: Create a Plex PIN and redirect user to Plex auth page
        app.MapGet("/auth/plex-start", async (PlexAuthService plexAuth, HttpContext context) =>
        {
            var pin = await plexAuth.CreatePinAsync();
            if (pin is null)
                return Results.Redirect("/login?error=Failed+to+create+Plex+PIN");

            // Store pinId in a temp cookie so the callback can find it
            context.Response.Cookies.Append("PlexPinId", pin.Value.PinId.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = !context.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment(),
                SameSite = SameSiteMode.Lax,
                MaxAge = TimeSpan.FromMinutes(5)
            });

            var callbackUrl = $"{context.Request.Scheme}://{context.Request.Host}/auth/plex-callback";
            var authUrl = plexAuth.GetAuthRedirectUrl(pin.Value.PinCode, callbackUrl);

            return Results.Redirect(authUrl);
        }).AllowAnonymous();

        // Step 2: Plex redirects back here after user authenticates
        app.MapGet("/auth/plex-callback", async (PlexAuthService plexAuth, HttpContext context) =>
        {
            if (!context.Request.Cookies.TryGetValue("PlexPinId", out var pinIdStr) || !int.TryParse(pinIdStr, out var pinId))
                return Results.Redirect("/login?error=Missing+PIN");

            // Clean up temp cookie
            context.Response.Cookies.Delete("PlexPinId");

            // Poll for the auth token (Plex may take a moment)
            string? authToken = null;
            for (var attempt = 0; attempt < 10; attempt++)
            {
                authToken = await plexAuth.CheckPinAsync(pinId);
                if (authToken is not null) break;
                await Task.Delay(1000);
            }

            if (authToken is null)
                return Results.Redirect("/login?error=Plex+authentication+timed+out");

            var plexUser = await plexAuth.GetUserAsync(authToken);
            if (plexUser is null)
                return Results.Redirect("/login?error=Failed+to+get+Plex+user+info");

            var hasAccess = await plexAuth.HasServerAccessAsync(authToken, plexUser.Id);
            if (!hasAccess)
                return Results.Redirect($"/login?error={Uri.EscapeDataString($"Sorry {plexUser.Username}, you don't have access to this server.")}");

            // Create auth cookie
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, plexUser.Id.ToString()),
                new(ClaimTypes.Name, plexUser.Username),
                new("PlexToken", authToken),
            };

            if (!string.IsNullOrEmpty(plexUser.Thumb))
                claims.Add(new Claim("PlexThumb", plexUser.Thumb));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
                });

            return Results.Redirect("/");
        }).AllowAnonymous();

        app.MapGet("/auth/logout", async (HttpContext context) =>
        {
            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Results.Redirect("/login");
        }).AllowAnonymous();
    }
}
