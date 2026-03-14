using CheapNights.Components;
using CheapNights.Data;
using CheapNights.Repositories;
using CheapNights.Services;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddDbContextFactory<HorrorDbContext>(opt =>
{
    if (builder.Environment.IsDevelopment())
    {
        opt.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=horror.db");
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

var app = builder.Build();

// Auto-create database on startup
await using (var scope = app.Services.CreateAsyncScope())
{
    var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<HorrorDbContext>>();
    await using var db = factory.CreateDbContext();
    await db.Database.EnsureCreatedAsync();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
