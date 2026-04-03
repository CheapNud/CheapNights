# Migration: Groups Feature + Theme Preset

## Current state
- Production PostgreSQL has `20260314213953_InitialMigration` applied
- Snapshot restored from `c5f2f32` and adapted for `CheapNightsDbContext`
- Backup of broken migration at `../CheapNights_Migrations_backup/`

## Step 1: Generate the migration

Run with Production environment so DesignTimeDbContextFactory uses Npgsql:

```bash
ASPNETCORE_ENVIRONMENT=Production dotnet ef migrations add GroupsAndThemePreset
```

This should generate an incremental migration with:
- CREATE TABLE: `AppUsers`, `Groups`, `GroupMembers`, `MemberGamePlatforms`
- ALTER `Categories`: add `GroupId` (FK)
- ALTER `GameEntries`: drop `PlatformBrechtId`/`PlatformPieterId`, add `GroupId` (FK)
- ALTER `NowPlaying`: add `GroupId` (FK + unique index)
- ALTER `PlannedSessions`: drop `Location`, add `GroupId` (FK) + `HostMemberId` (FK)

## Step 2: Add data migration SQL to the generated Up()

After EF generates the schema changes, add this SQL **before** any NOT NULL constraints
or FK constraints are applied, and **after** the new tables are created.

The order matters — insert data before adding NOT NULL constraints on GroupId columns.

### Strategy: two-phase column addition
EF will likely add GroupId as nullable first (since existing rows need values),
then you set the data, then alter to NOT NULL. If EF generates it as NOT NULL directly,
you'll need to restructure: add as nullable → populate → alter to NOT NULL.

### Data migration SQL to inject:

```csharp
// After Groups table is created, before GroupId FKs are enforced:

// 1. Create the default group (Horror Nights — all existing data belongs here)
migrationBuilder.Sql(@"
    INSERT INTO ""Groups"" (""Name"", ""Description"", ""ThemeColor"", ""ThemePreset"", ""IconName"", ""CreatedAt"")
    VALUES ('Horror Nights', 'Survival horror roadmap — Resident Evil & Silent Hill',
            '#c0392b', 'horror-dark', 'Theaters', NOW());
");

// 2. Create AppUsers for known users
migrationBuilder.Sql(@"
    INSERT INTO ""AppUsers"" (""PlexUserId"", ""DisplayName"", ""CreatedAt"")
    VALUES ('brecht', 'Brecht', NOW()),
           ('pieter', 'Pieter', NOW());
");

// 3. Create GroupMembers (linking users to the Horror Nights group)
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

// 4. Set GroupId on existing data (all belongs to Horror Nights)
migrationBuilder.Sql(@"
    UPDATE ""Categories"" SET ""GroupId"" = (SELECT ""Id"" FROM ""Groups"" WHERE ""Name"" = 'Horror Nights');
    UPDATE ""GameEntries"" SET ""GroupId"" = (SELECT ""Id"" FROM ""Groups"" WHERE ""Name"" = 'Horror Nights');
    UPDATE ""NowPlaying"" SET ""GroupId"" = (SELECT ""Id"" FROM ""Groups"" WHERE ""Name"" = 'Horror Nights');
    UPDATE ""PlannedSessions"" SET ""GroupId"" = (SELECT ""Id"" FROM ""Groups"" WHERE ""Name"" = 'Horror Nights');
");

// 5. Migrate PlatformBrechtId/PlatformPieterId → MemberGamePlatforms
//    (before dropping those columns!)
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
//    (before dropping Location column!)
migrationBuilder.Sql(@"
    UPDATE ""PlannedSessions""
    SET ""HostMemberId"" = (SELECT ""Id"" FROM ""GroupMembers"" WHERE ""Nickname"" = LOWER(""Location""))
    WHERE ""Location"" IS NOT NULL AND ""Location"" != '';
");
```

### Ordering in Up() method:
1. CreateTable for `AppUsers`, `Groups`, `GroupMembers`, `MemberGamePlatforms`
2. AddColumn `GroupId` (nullable!) on Categories, GameEntries, NowPlaying, PlannedSessions
3. AddColumn `HostMemberId` (nullable) on PlannedSessions
4. **INSERT data migration SQL (steps 1-6 above)**
5. **ALTER columns to NOT NULL** where needed (GroupId on Categories, GameEntries)
6. DropColumn `PlatformBrechtId`, `PlatformPieterId` from GameEntries
7. DropColumn `Location` from PlannedSessions
8. CreateIndex / AddForeignKey

## Step 3: Test locally first

Before touching production:
1. Take a PostgreSQL dump: `pg_dump -Fc cheapnights > cheapnights_backup.dump`
2. Create a test database from the dump: `pg_restore -d cheapnights_test cheapnights_backup.dump`
3. Run migration against test: `ASPNETCORE_ENVIRONMENT=Production dotnet ef database update`
4. Verify data is intact

## Rollback plan
- Backup at `../CheapNights_Migrations_backup/`
- PostgreSQL dump before migration
- `pg_restore` to revert if anything goes wrong
