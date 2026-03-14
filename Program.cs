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
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=horror.db"));

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
    await using var db = await factory.CreateDbContextAsync();
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
