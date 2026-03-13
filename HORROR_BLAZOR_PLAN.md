# Horror Roadmap — Blazor App Plan
**Target**: `horror.cheapludes.be` hosted on **Megaton** (app server) behind **Hidden-Valley** (nginx)  
**Stack**: Blazor Server · MudBlazor 8.14.0 · EF Core · SQLite · ASP.NET Core  
**POC reference**: `horror-roadmap.html` — port the design and UX directly

---

## Context

This is a shared co-op horror game tracker for two players: **Brecht** and **Pieter**.  
The HTML POC proved the UX. The Blazor app adds shared state — one database, both players see the same data in real time via Blazor Server's SignalR connection.

**Key problems solved over the POC:**
- `localStorage` is per-browser — completion/sessions/now-playing need to be shared
- Completion tracking needs a real backend
- Real-time updates: Pieter marks RE7 done → Brecht sees it instantly

---

## Project Setup

```bash
dotnet new blazorserver -n HorrorRoadmap
cd HorrorRoadmap
dotnet add package MudBlazor --version 8.14.0
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

**DO NOT run `dotnet ef migrations add` or `dotnet ef database update`** — migrations are handled separately.

Add to `Program.cs`:
```csharp
builder.Services.AddMudServices();
builder.Services.AddDbContextFactory<HorrorDbContext>(opt =>
    opt.UseSqlite("Data Source=horror.db"));
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<RoadmapService>();
builder.Services.AddSingleton<NowPlayingService>();
```

Add to `_Imports.razor`:
```razor
@using MudBlazor
@using HorrorRoadmap.Models
@using HorrorRoadmap.Services
```

Add to `App.razor` / layout: MudBlazor theme provider, dark theme matching the HTML POC.

---

## Folder Structure

```
HorrorRoadmap/
├── Models/
│   ├── GameEntry.cs
│   ├── PlannedSession.cs
│   └── NowPlaying.cs
├── Data/
│   └── HorrorDbContext.cs
├── Services/
│   ├── RoadmapService.cs
│   ├── SessionService.cs
│   └── NowPlayingService.cs
├── Components/
│   ├── NowPlayingBar.razor
│   ├── RoadmapTable.razor
│   ├── SessionCalendar.razor
│   ├── SessionModal.razor
│   └── NowPlayingDialog.razor
├── Pages/
│   └── Index.razor          ← single-page app, all components compose here
└── wwwroot/
    └── app.css              ← port horror theme from POC
```

---

## Models

### `GameEntry.cs`
```csharp
public class GameEntry
{
    public int Id { get; set; }
    public required string Name { get; set; }        // "RE7 BIOHAZARD"
    public required string Category { get; set; }    // "RE" | "SH"
    public required string Type { get; set; }        // "Game" | "Movie" | "DLC"
    public string? Protagonist { get; set; }
    public string? StoryEra { get; set; }
    public string? Status { get; set; }              // "Essential" | "Recommended" | "Skip" | "Upcoming" | "Done"
    public int StarRating { get; set; }              // 1-3
    public string? LengthLabel { get; set; }         // "~9h"
    public string? PlatformBrecht { get; set; }
    public string? PlatformPieter { get; set; }
    public bool IsCouchCoop { get; set; }
    public bool IsMovie { get; set; }
    public int SortOrder { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? CompletedNote { get; set; }       // "Finished in 2.5 nights, amazing ending"
}
```

### `PlannedSession.cs`
```csharp
public class PlannedSession
{
    public int Id { get; set; }
    public DateTime ScheduledAt { get; set; }        // Full DateTime incl. time (e.g. 2026-03-20 20:00)
    public required string Location { get; set; }    // "brecht" | "pieter"
    public int? GameEntryId { get; set; }
    public GameEntry? GameEntry { get; set; }
    public string? CustomGame { get; set; }          // fallback if no GameEntry match
    public string? Notes { get; set; }               // "Pizza ordered, bring snacks"
    public bool IsCompleted { get; set; }
}
```

### `NowPlaying.cs`
```csharp
public class NowPlaying
{
    public int Id { get; set; }                      // always Id = 1, single row
    public int? GameEntryId { get; set; }
    public GameEntry? GameEntry { get; set; }
    public string? StatusNote { get; set; }          // "almost finished, Arc reached"
    public DateTime UpdatedAt { get; set; }
}
```

---

## Database Context

```csharp
public class HorrorDbContext(DbContextOptions<HorrorDbContext> options) : DbContext(options)
{
    public DbSet<GameEntry> GameEntries => Set<GameEntry>();
    public DbSet<PlannedSession> PlannedSessions => Set<PlannedSession>();
    public DbSet<NowPlaying> NowPlaying => Set<NowPlaying>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed all game entries from the HTML POC in correct sort order
        // See "Data Seeding" section below
        modelBuilder.Entity<NowPlaying>().HasData(
            new NowPlaying { Id = 1, StatusNote = null, UpdatedAt = DateTime.UtcNow }
        );
    }
}
```

---

## Services

### `NowPlayingService.cs` — Singleton for real-time broadcast
```csharp
public class NowPlayingService
{
    // Broadcast to all connected Blazor Server circuits when Now Playing changes
    public event Action? OnChange;
    public void NotifyChanged() => OnChange?.Invoke();
}
```

### `RoadmapService.cs`
```csharp
public class RoadmapService(IDbContextFactory<HorrorDbContext> factory)
{
    public async Task<List<GameEntry>> GetAllAsync() { ... }
    public async Task<List<GameEntry>> GetByCategoryAsync(string category) { ... }
    public async Task MarkCompletedAsync(int id, string? note) { ... }
    public async Task UnmarkCompletedAsync(int id) { ... }
}
```

### `SessionService.cs`
```csharp
public class SessionService(IDbContextFactory<HorrorDbContext> factory)
{
    public async Task<List<PlannedSession>> GetSessionsForMonthAsync(int year, int month) { ... }
    public async Task<List<PlannedSession>> GetUpcomingAsync(int count = 8) { ... }
    public async Task<PlannedSession?> GetByDateAsync(DateTime date) { ... }
    public async Task SaveSessionAsync(PlannedSession session) { ... }
    public async Task DeleteSessionAsync(int id) { ... }
}
```

---

## Components

### `NowPlayingBar.razor`
- Replicates the POC bar exactly: pulsing blue dot, game name, status note, ⚙ icon
- ⚙ opens `NowPlayingDialog`
- Subscribes to `NowPlayingService.OnChange` → `StateHasChanged()` for real-time updates
- Use `IDisposable` to unsubscribe on dispose

```razor
@implements IDisposable

protected override void OnInitialized()
{
    NowPlayingService.OnChange += StateHasChanged;
}
public void Dispose()
{
    NowPlayingService.OnChange -= StateHasChanged;
}
```

### `RoadmapTable.razor`
- Two `MudTable` instances: RE and SH
- Completion: checkbox or click-to-toggle on each row
- Completed rows get a visual strike/dim treatment (like the POC skip rows)
- Movie rows stay slim/dimmed as in the POC
- Parameters: `Category` ("RE"/"SH"), `GameEntries`

### `SessionCalendar.razor`
- Port the POC calendar grid
- Use `MudIconButton` for nav arrows
- Day cells as `MudPaper` components
- Click → open `SessionModal`
- Receives sessions from `SessionService`

### `SessionModal.razor`
- `MudDialog` 
- DateTime picker (MudDatePicker + MudTimePicker)  
- Game dropdown: `MudSelect<GameEntry>` — defaults to current NowPlaying game
- Location: `MudToggleGroup` or two `MudButton` toggles (Brecht / Pieter)
- Notes: `MudTextField` multiline
- Save / Delete actions

### `NowPlayingDialog.razor`
- `MudDialog`
- `MudSelect<GameEntry>` for game
- `MudTextField` for status note
- On save: update DB + call `NowPlayingService.NotifyChanged()`

---

## Data Seeding

Seed `GameEntries` in `OnModelCreating` with all entries from the POC in order.  
Reference the HTML for exact names, categories, sort orders, star ratings, platform info.

Example seed structure (add all ~25 entries):
```csharp
modelBuilder.Entity<GameEntry>().HasData(
    new GameEntry { Id = 1, SortOrder = 0, Name = "RE9 REQUIEM", Category = "RE", Type = "Game",
        Protagonist = "Grace + Leon", StoryEra = "2026", Status = "Done",
        StarRating = 3, LengthLabel = "~15h", PlatformPieter = "PS5",
        IsCompleted = false },
    new GameEntry { Id = 2, SortOrder = 1, Name = "RE7 BIOHAZARD", Category = "RE", Type = "Game",
        Protagonist = "Ethan Winters", StoryEra = "2017", Status = "Essential",
        StarRating = 2, LengthLabel = "~9h", PlatformPieter = "PS5" },
    // ... continue for all entries
);
```

---

## Theming

Port the horror CSS variables to a MudBlazor dark theme:

```csharp
var horrorTheme = new MudTheme
{
    PaletteLight = new PaletteLight { ... },
    PaletteDark = new PaletteDark
    {
        Primary = "#c0392b",        // RE red
        Secondary = "#2a9d8f",      // SH teal
        Background = "#080808",
        Surface = "#0f0f0f",
        AppbarBackground = "#080808",
        TextPrimary = "#e0e0e0",
        TextSecondary = "#666666",
    },
    Typography = new Typography
    {
        Default = new Default { FontFamily = ["Share Tech Mono", "monospace"] },
        H1 = new H1 { FontFamily = ["Bebas Neue", "sans-serif"] },
    }
};
```

Fonts: keep the Google Fonts imports from the POC in `wwwroot/index.html`.  
Custom scanline effect, card borders etc. in `wwwroot/app.css`.

---

## Deployment on Megaton

### Dockerfile
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HorrorRoadmap.dll"]
```

### docker-compose.yml
```yaml
services:
  horror-roadmap:
    build: .
    container_name: horror-roadmap
    restart: unless-stopped
    volumes:
      - ./data:/app/data          # SQLite persistence
    environment:
      - ASPNETCORE_URLS=http://+:5000
      - ConnectionStrings__Default=Data Source=/app/data/horror.db
    ports:
      - "5010:5000"               # pick a free port on Megaton
```

### Hidden-Valley nginx vhost
```nginx
server {
    listen 80;
    server_name horror.cheapludes.be;

    location / {
        proxy_pass http://megaton:5010;     # or Megaton's LAN IP
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection "upgrade";   # required for SignalR/WebSocket
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_read_timeout 3600s;                # long timeout for Blazor Server keep-alive
    }
}
```

**The `Upgrade` / `Connection` headers are critical** — Blazor Server uses WebSockets for its SignalR circuit. Without them the app will load but immediately disconnect.

---

## Build Order for Claude Code

1. **Project scaffold** — `dotnet new`, packages, folder structure
2. **Models** — all three model classes
3. **DbContext** — with full seed data from the POC
4. **Services** — RoadmapService, SessionService, NowPlayingService
5. **Theme** — MudBlazor dark theme + import POC fonts/CSS
6. **NowPlayingBar** — static first, then wire to service
7. **RoadmapTable** — RE table first, then SH, then completion toggle
8. **SessionCalendar** — grid render first, then click-to-open
9. **SessionModal** — full form with NowPlaying default
10. **NowPlayingDialog** — with real-time broadcast
11. **Index.razor** — compose all components
12. **Dockerfile + docker-compose** — ready to deploy
13. **nginx config snippet** — for Hidden-Valley

---

## Notes for Claude Code

- **Primary constructors** everywhere (`C# 12+`)
- **Collection expressions** for lists
- **`IDbContextFactory<T>`** not `DbContext` directly — Blazor Server has scoping issues with DbContext singleton
- **Never `AddAsync()`** — use `Add()`, async only at `SaveChangesAsync()`
- **Lambda LINQ only** — no query syntax ever
- **`Debug.WriteLine()`** not `Console.WriteLine()`
- **MudBlazor components** over raw HTML — no `<img>`, `<p>` etc where MudBlazor alternatives exist
- **`T=` parameter** always explicit on `MudList`, `MudSelect`, `MudChip`
- **No `async void`** — use `Func<Task>` for async event handlers
- **FluentValidation** on `PlannedSession` form inputs
- **No migrations executed** — generate migration files only, do not run them
- The POC HTML file is the design bible — match it as closely as possible
