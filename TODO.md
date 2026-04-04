<!--
  TODO.md — CheapNights project work tracker
  Last updated: 2026-03-29

  RULES FOR AI AGENTS:
  - Update the "Last updated" date above whenever you modify this file
  - Items use checkbox format: - [ ] incomplete, - [x] complete
  - Never remove completed items — they serve as history. Move them to "## Done" when a category gets cluttered.
  - Each item gets ONE line. Details go in sub-bullets indented with 2 spaces.
  - Prefix each item with the date it was added: - [ ] (2026-03-17) Description
  - When completing, change to: - [x] (2026-03-17 → 2026-03-18) Description
  - Tag the SOURCE of each item at the end in brackets:
      [code-todo] = from // TODO comment in source code
      [plan] = from a plan document or planning session
      [bug] = from a bug encountered during dev/deploy
      [audit] = from a code audit or review
      [user] = explicitly requested by the user
  - For [code-todo] items, ALWAYS include file:line reference so devs can navigate directly
  - Categories: Blocking, Planned, Future, Done
  - New items go at the TOP of their category
  - Do not create separate TODO_*.md files — everything goes here
  - Keep it terse. If it needs more than 3 sub-bullets, link to a plan document.
  - Do NOT create, rename, or remove categories — the fixed set is: Blocking, Planned, Future, Done
  - When asked for planned work or TODO analysis, ALWAYS include Future items too — list them below Planned and note them as future work
-->

# TODO

## Blocking

- [ ] (2026-03-28) Migrate local Plex auth to CheapHelpers shared provider [user]
  - Remove `Services/PlexAuthService.cs`, `Helpers/AuthEndpoints.cs`, `Helpers/PlexConstants.cs`
  - Remove `DTOs/PlexPinResponse.cs`, `DTOs/PlexUserInfo.cs`
  - Add CheapHelpers.Services + CheapHelpers.Blazor references
  - Wire `AddPlexAuth()` + `MapPlexAuthEndpoints()` in Program.cs with `AuthorizeUser` hook for server access gating

## Planned

- [ ] (2026-03-29) Create production migration for multi-group schema [plan]
  - Old migrations deleted (schema fundamentally changed)
  - Need fresh migration before deploying to PostgreSQL
  - Dev uses EnsureCreated (auto-recreates from new context)
  - Requires careful data migration script for existing prod data

- [ ] (2026-03-29) Wire Plex auth to create/lookup AppUser on login [plan]
  - AuthEndpoints callback should call `AppUserRepo.GetOrCreateAsync()` with Plex claims
  - Store AppUser.Id in claims for downstream group lookups
  - Currently seeded with placeholder PlexUserIds ("brecht", "pieter", "miel") — replace with real Plex IDs

- [ ] (2026-03-31) Add all live-action Resident Evil films to GameEntry seed data in current sort order [user]
  - Paul W.S. Anderson series (2002-2016): RE, Apocalypse, Extinction, Afterlife, Retribution, The Final Chapter
  - Welcome to Raccoon City (2021)

- [ ] (2026-03-29) Add CSS vars for Schedule 1 badge color [plan]
  - `--s1-accent` color var in app.css for the S1 category badge
  - Currently only `--re-red` and `--sh-teal` exist

## Future

- [ ] (2026-03-29) Group invitation system (share link or Plex friend picker) [plan]
- [ ] (2026-03-29) Per-group notification preferences [plan]
- [ ] (2026-03-29) Group activity feed / history log [plan]
- [ ] (2026-03-29) Archive/soft-delete completed groups [plan]

## Done

- [x] (2026-03-29 → 2026-03-29) Phase 1: Create AppUser, Group, GroupMember, MemberGamePlatform entities [plan]
- [x] (2026-03-29 → 2026-03-29) Phase 2: Add GroupId FKs to GameEntry, Category, PlannedSession, NowPlaying + migrate seed data [plan]
- [x] (2026-03-29 → 2026-03-29) Phase 3: Rename HorrorDbContext → CheapNightsDbContext, drop PlatformBrechtId/PieterId, drop Location, remove Players constants [plan]
- [x] (2026-03-29 → 2026-03-29) Phase 4: Refactor all repos/services to be group-scoped, create GroupRepo + AppUserRepo + ActiveGroupService [plan]
- [x] (2026-03-29 → 2026-03-29) Phase 5: UI refactoring — group selector in layout, dynamic titles/members/platforms/locations across all components [plan]
- [x] (2026-03-29 → 2026-03-29) Phase 6: Group management page (/groups) with create/edit dialogs, Schedule 1 group seeded with Brecht + Miel [plan]
