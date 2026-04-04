# Migration: Groups Feature + Theme Preset

## Context

The groups feature (`31b7ca6`) deleted the old `InitialMigration` without generating a
replacement. The old snapshot has been restored from `c5f2f32` and adapted for
`CheapNightsDbContext`. Production PostgreSQL still has `20260314213953_InitialMigration`
applied — the pre-groups schema.

Broken migration backup: `../CheapNights_Migrations_backup/`

---

## Next Steps (in order)

### 1. Backup production database

```bash
pg_dump -Fc cheapnights > cheapnights_pre_groups_backup.dump
```

### 2. Generate incremental migration

DesignTimeDbContextFactory uses Npgsql when environment != Development.
This makes EF diff the old snapshot (pre-groups Npgsql schema) against the current model
and generate proper ALTER/CREATE statements with PostgreSQL types.

```bash
ASPNETCORE_ENVIRONMENT=Production dotnet ef migrations add GroupsAndThemePreset
```

Expected output — an incremental migration containing:
- **CREATE TABLE**: `AppUsers`, `Groups`, `GroupMembers`, `MemberGamePlatforms`
- **ALTER** `Categories`: add `GroupId` (FK)
- **ALTER** `GameEntries`: drop `PlatformBrechtId`/`PlatformPieterId`, add `GroupId` (FK)
- **ALTER** `NowPlaying`: add `GroupId` (FK + unique index)
- **ALTER** `PlannedSessions`: drop `Location`, add `GroupId` (FK) + `HostMemberId` (FK)

If the migration instead generates full CREATE TABLE for everything — the snapshot
restoration didn't work. Do NOT apply it. Investigate first.

### 3. Restructure the generated Up() for data preservation

EF doesn't know about existing data. The generated migration will likely try to add
`GroupId` as NOT NULL directly, which fails on existing rows. Restructure `Up()` to:

**Phase 1 — Create new tables**
```
CreateTable AppUsers
CreateTable Groups
CreateTable GroupMembers
CreateTable MemberGamePlatforms
```

**Phase 2 — Add new columns as NULLABLE**

If EF generated them as `nullable: false`, change to `nullable: true` temporarily:
```
AddColumn GroupId (nullable!) on Categories, GameEntries, NowPlaying, PlannedSessions
AddColumn HostMemberId (nullable) on PlannedSessions
```

**Phase 3 — Migrate data**

Insert this after new columns exist but before constraints are enforced:

```csharp
// 1. Create default group — all existing data belongs to Horror Nights
migrationBuilder.Sql(@"
    INSERT INTO ""Groups"" (""Name"", ""Description"", ""ThemeColor"", ""ThemePreset"", ""IconName"", ""CreatedAt"")
    VALUES ('Horror Nights', 'Survival horror roadmap — Resident Evil & Silent Hill',
            '#c0392b', 'horror-dark', 'Theaters', NOW());
");

// 2. Create AppUsers for existing known users
migrationBuilder.Sql(@"
    INSERT INTO ""AppUsers"" (""PlexUserId"", ""DisplayName"", ""CreatedAt"")
    VALUES ('brecht', 'Brecht', NOW()),
           ('pieter', 'Pieter', NOW());
");

// 3. Create GroupMembers linking users to Horror Nights
migrationBuilder.Sql(@"
    INSERT INTO ""GroupMembers"" (""GroupId"", ""AppUserId"", ""Nickname"")
    VALUES (
        (SELECT ""Id"" FROM ""Groups"" WHERE ""Name"" = 'Horror Nights'),
        (SELECT ""Id"" FROM ""AppUsers"" WHERE ""PlexUserId"" = 'brecht'),
        'brecht'
    ),
    (
        (SELECT ""Id"" FROM ""Groups"" WHERE ""Name"" = 'Horror Nights'),
        (SELECT ""Id"" FROM ""AppUsers"" WHERE ""PlexUserId"" = 'pieter'),
        'pieter'
    );
");

// 4. Populate GroupId on all existing rows
migrationBuilder.Sql(@"
    UPDATE ""Categories"" SET ""GroupId"" = (SELECT ""Id"" FROM ""Groups"" WHERE ""Name"" = 'Horror Nights');
    UPDATE ""GameEntries"" SET ""GroupId"" = (SELECT ""Id"" FROM ""Groups"" WHERE ""Name"" = 'Horror Nights');
    UPDATE ""NowPlaying"" SET ""GroupId"" = (SELECT ""Id"" FROM ""Groups"" WHERE ""Name"" = 'Horror Nights');
    UPDATE ""PlannedSessions"" SET ""GroupId"" = (SELECT ""Id"" FROM ""Groups"" WHERE ""Name"" = 'Horror Nights');
");

// 5. Migrate PlatformBrechtId/PlatformPieterId → MemberGamePlatforms
//    MUST happen BEFORE dropping those columns
migrationBuilder.Sql(@"
    INSERT INTO ""MemberGamePlatforms"" (""GroupMemberId"", ""GameEntryId"", ""PlatformId"")
    SELECT
        (SELECT ""Id"" FROM ""GroupMembers"" WHERE ""Nickname"" = 'brecht'),
        ""Id"",
        ""PlatformBrechtId""
    FROM ""GameEntries""
    WHERE ""PlatformBrechtId"" IS NOT NULL;

    INSERT INTO ""MemberGamePlatforms"" (""GroupMemberId"", ""GameEntryId"", ""PlatformId"")
    SELECT
        (SELECT ""Id"" FROM ""GroupMembers"" WHERE ""Nickname"" = 'pieter'),
        ""Id"",
        ""PlatformPieterId""
    FROM ""GameEntries""
    WHERE ""PlatformPieterId"" IS NOT NULL;
");

// 6. Convert PlannedSessions.Location (string) → HostMemberId (FK)
//    MUST happen BEFORE dropping Location column
migrationBuilder.Sql(@"
    UPDATE ""PlannedSessions""
    SET ""HostMemberId"" = (SELECT ""Id"" FROM ""GroupMembers"" WHERE ""Nickname"" = LOWER(""Location""))
    WHERE ""Location"" IS NOT NULL AND ""Location"" != '';
");
```

**Phase 4 — Enforce constraints**

Now that data is populated, tighten the schema:
```
AlterColumn GroupId on Categories, GameEntries → NOT NULL
AlterColumn GroupId on NowPlaying, PlannedSessions → NOT NULL
DropColumn PlatformBrechtId from GameEntries
DropColumn PlatformPieterId from GameEntries
DropColumn Location from PlannedSessions
CreateIndex / AddForeignKey (as generated)
```

### 4. Test against a cloned database

```bash
pg_restore -C -d postgres cheapnights_pre_groups_backup.dump   # creates cheapnights_test
# point connection string at cheapnights_test temporarily
ASPNETCORE_ENVIRONMENT=Production dotnet ef database update
# verify: SELECT * FROM "Groups"; SELECT * FROM "GameEntries" WHERE "GroupId" IS NULL;
```

### 5. Apply to production

```bash
ASPNETCORE_ENVIRONMENT=Production dotnet ef database update
```

### 6. Clean up

- Delete `../CheapNights_Migrations_backup/` once confirmed working
- Delete this file (`MIGRATION_NOTES.md`)
- Dev SQLite: delete `horror.db` and let `EnsureCreated` rebuild with new schema

---

## Rollback plan

- `pg_restore -c -d cheapnights cheapnights_pre_groups_backup.dump`
- Revert migration files from backup
