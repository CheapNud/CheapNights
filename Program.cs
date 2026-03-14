using CheapNights.Components;
using CheapNights.Data;
using CheapNights.Helpers;
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
builder.Services.AddHttpClient();
builder.Services.AddDbContextFactory<HorrorDbContext>(opt =>
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
builder.Services.AddScoped<RoadmapService>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddSingleton<NowPlayingService>();
builder.Services.AddSingleton<PlexAuthService>();

var app = builder.Build();

// Dev: EnsureCreated (delete horror.db to reset schema + seed data)
// Prod: MigrateAsync applies pending migrations on startup
await using (var scope = app.Services.CreateAsyncScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<HorrorDbContext>>();
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

app.MapAuthEndpoints();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
