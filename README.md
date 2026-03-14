# CheapNights

Co-op horror game night planner — track franchise roadmaps, plan sessions, and coordinate playthroughs with friends.

## Features

- **Franchise Roadmaps** — Resident Evil and Silent Hill with game/movie/DLC entries, status tracking, star ratings, and platform ownership
- **Session Planner** — Monthly calendar with session scheduling, location toggle, and upcoming session list
- **Now Playing** — Live tracker showing the currently active game with auto-completion when switching
- **Plex SSO** — Sign in with Plex, restricted to server friends only
- **Entity-driven UI** — Statuses, categories, entry types, and platforms are all database entities with data-driven chip styling
- **CheapHelpers Integration** — BaseRepo, Selector components, AbsoluteExpirationCache, UriExtensions

## Tech Stack

- **C# 14 / .NET 11** — Blazor Server with interactive rendering
- **MudBlazor 9.1** — Component library and theming
- **EF Core 10** — SQLite (dev) / PostgreSQL (prod)
- **CheapHelpers** — EF repos, Blazor selectors, caching
- **Docker** — GHCR publishing via GitHub Actions
- **Plex API** — PIN-based authentication (3 HTTP calls, no external packages)

## Development

```bash
# Run locally (SQLite, auto-creates horror.db)
dotnet run

# Plex admin token (required)
dotnet user-secrets set "Plex:AdminToken" "your-token"
```

## Deployment

Published to `ghcr.io/cheapnud/cheapnights:latest` on push to `main`.

```bash
# On host
docker compose pull && docker compose up -d
```

Requires `.env` with `PG_PASSWORD` and `PLEX_ADMIN_TOKEN`.

## CI/CD

- **publish.yml** — Docker build + push to GHCR
- **pr-review.yml** — Claude Sonnet 4.6 code review
- **codeql.yml** — Security and quality scanning
- **dependency-review.yml** — License and vulnerability check
- **dotnet.yml** — Build validation

## Future Roadmap

### Notifications
- Email notifications via CheapHelpers mailing services
- Push notifications (free tier)
- Session reminders and game completion alerts

### Management Pages
- Create and manage game groups with custom theming
- Admin dashboard for entry management
- Bulk operations for status updates

### Expanded Auth Providers
- Microsoft SSO
- Google OAuth
- Apple Sign In
- ASP.NET Identity integration

### Metadata Enrichment
- Steam / SteamDB integration for game details and pricing
- IMDB / Rotten Tomatoes for movie ratings
- TVDB / TheMovieDB for additional metadata
- Plex library integration for ownership detection

## License

MIT
