using CheapHelpers.Blazor.Extensions;
using CheapHelpers.Services.Auth.Plex;
using CheapHelpers.Services.Auth.Plex.Extensions;
using CheapNights.Components;
using CheapNights.Data;
using CheapNights.Repositories;
using CheapNights.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => cfg
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.Console()
    .Enrich.WithProperty("Application", "CheapNights"));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(opt => opt.DetailedErrors = builder.Environment.IsDevelopment());

builder.Services.AddMudServices();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath = "/login";
        opt.ExpireTimeSpan = TimeSpan.FromDays(30);
        opt.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddPlexAuth(opts =>
{
    opts.ProductName = "CheapNights";
    opts.ClientIdentifier = builder.Configuration["Plex:ClientId"] ?? "CheapNights";
    opts.AdminToken = builder.Configuration["Plex:AdminToken"];
    opts.CallbackBaseUrl = builder.Configuration["Plex:CallbackBaseUrl"];
    opts.PinPollAttempts = 5;
    opts.PinPollDelay = TimeSpan.FromSeconds(1);
    opts.PostLogoutRedirect = "/login";
    opts.AuthorizeUser = async (plexUser, sp, ct) =>
    {
        var plexAuth = sp.GetRequiredService<IPlexAuthService>();
        if (!await plexAuth.HasServerAccessAsync(plexUser.Id, ct))
            return false;

        var appUserRepo = sp.GetRequiredService<AppUserRepo>();
        await appUserRepo.GetOrCreateAsync(
            plexUser.Id.ToString(),
            plexUser.Username,
            plexUser.Thumb);

        return true;
    };
});
builder.Services.AddDbContextFactory<CheapNightsDbContext>(opt =>
{
    if (builder.Environment.IsDevelopment())
    {
        opt.UseSqlite("Data Source=horror.db");
    }
    else
    {
        var pgConnection = builder.Configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("PostgreSQL connection string 'Default' is required in production.");
        opt.UseNpgsql(pgConnection);
    }
});

builder.Services.AddScoped<GameEntryRepo>();
builder.Services.AddScoped<SessionRepo>();
builder.Services.AddScoped<NowPlayingRepo>();
builder.Services.AddScoped<StatusRepo>();
builder.Services.AddScoped<GroupRepo>();
builder.Services.AddScoped<AppUserRepo>();
builder.Services.AddScoped<RoadmapService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<ActiveGroupService>();
builder.Services.AddSingleton<NowPlayingService>();

var app = builder.Build();

// Dev: EnsureCreated (delete horror.db to reset schema + seed data)
// Prod: MigrateAsync applies pending migrations on startup
await using (var scope = app.Services.CreateAsyncScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<CheapNightsDbContext>>();
    await using var db = factory.CreateDbContext();

    if (app.Environment.IsDevelopment())
        await db.Database.EnsureCreatedAsync();
    else
        await db.Database.MigrateAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapPlexAuthEndpoints();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
