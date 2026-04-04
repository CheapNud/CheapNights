using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CheapNights.Migrations
{
    /// <inheritdoc />
    public partial class GroupsAndThemePreset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ──────────────────────────────────────────────────
            // Phase 1 — Create new tables
            // ──────────────────────────────────────────────────

            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlexUserId = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    AvatarUrl = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ThemeColor = table.Column<string>(type: "text", nullable: true),
                    ThemePreset = table.Column<string>(type: "text", nullable: false),
                    IconName = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupId = table.Column<int>(type: "integer", nullable: false),
                    AppUserId = table.Column<int>(type: "integer", nullable: false),
                    Nickname = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupMembers_AppUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupMembers_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MemberGamePlatforms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GroupMemberId = table.Column<int>(type: "integer", nullable: false),
                    GameEntryId = table.Column<int>(type: "integer", nullable: false),
                    PlatformId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MemberGamePlatforms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MemberGamePlatforms_GameEntries_GameEntryId",
                        column: x => x.GameEntryId,
                        principalTable: "GameEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberGamePlatforms_GroupMembers_GroupMemberId",
                        column: x => x.GroupMemberId,
                        principalTable: "GroupMembers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MemberGamePlatforms_Platforms_PlatformId",
                        column: x => x.PlatformId,
                        principalTable: "Platforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_AppUserId",
                table: "GroupMembers",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembers_GroupId_AppUserId",
                table: "GroupMembers",
                columns: new[] { "GroupId", "AppUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberGamePlatforms_GameEntryId",
                table: "MemberGamePlatforms",
                column: "GameEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_MemberGamePlatforms_GroupMemberId_GameEntryId",
                table: "MemberGamePlatforms",
                columns: new[] { "GroupMemberId", "GameEntryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MemberGamePlatforms_PlatformId",
                table: "MemberGamePlatforms",
                column: "PlatformId");

            // ──────────────────────────────────────────────────
            // Phase 2 — Add new columns as NULLABLE
            // ──────────────────────────────────────────────────

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "Categories",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "GameEntries",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "NowPlaying",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "PlannedSessions",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HostMemberId",
                table: "PlannedSessions",
                type: "integer",
                nullable: true);

            // ──────────────────────────────────────────────────
            // Phase 3 — Seed reference data + migrate existing rows
            // ──────────────────────────────────────────────────

            // Insert seed Groups
            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "CreatedAt", "Description", "IconName", "Name", "ThemeColor", "ThemePreset" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 3, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Survival horror roadmap — Resident Evil & Silent Hill", "Theaters", "Horror Nights", "#c0392b", "horror-dark" },
                    { 2, new DateTime(2026, 3, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Schedule 1 gaming sessions", "SportsEsports", "Schedule 1", "#4caf50", "forest" }
                });

            // Insert seed AppUsers
            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "AvatarUrl", "CreatedAt", "DisplayName", "PlexUserId" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 3, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Brecht", "brecht" },
                    { 2, null, new DateTime(2026, 3, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Pieter", "pieter" },
                    { 3, null, new DateTime(2026, 3, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Miel", "miel" }
                });

            // Insert seed GroupMembers
            migrationBuilder.InsertData(
                table: "GroupMembers",
                columns: new[] { "Id", "AppUserId", "GroupId", "Nickname" },
                values: new object[,]
                {
                    { 1, 1, 1, "brecht" },
                    { 2, 2, 1, "pieter" },
                    { 3, 1, 2, "brecht" },
                    { 4, 3, 2, "miel" }
                });

            // Assign ALL existing rows to Horror Nights (Group 1) — catches user-added data beyond seeds
            migrationBuilder.Sql(@"
                UPDATE ""Categories"" SET ""GroupId"" = 1 WHERE ""GroupId"" IS NULL;
                UPDATE ""GameEntries"" SET ""GroupId"" = 1 WHERE ""GroupId"" IS NULL;
                UPDATE ""NowPlaying"" SET ""GroupId"" = 1 WHERE ""GroupId"" IS NULL;
                UPDATE ""PlannedSessions"" SET ""GroupId"" = 1 WHERE ""GroupId"" IS NULL;
            ");

            // Migrate PlatformBrechtId → MemberGamePlatforms (brecht = GroupMember 1)
            // MUST happen BEFORE dropping those columns
            migrationBuilder.Sql(@"
                INSERT INTO ""MemberGamePlatforms"" (""GroupMemberId"", ""GameEntryId"", ""PlatformId"")
                SELECT 1, ""Id"", ""PlatformBrechtId""
                FROM ""GameEntries""
                WHERE ""PlatformBrechtId"" IS NOT NULL;
            ");

            // Migrate PlatformPieterId → MemberGamePlatforms (pieter = GroupMember 2)
            migrationBuilder.Sql(@"
                INSERT INTO ""MemberGamePlatforms"" (""GroupMemberId"", ""GameEntryId"", ""PlatformId"")
                SELECT 2, ""Id"", ""PlatformPieterId""
                FROM ""GameEntries""
                WHERE ""PlatformPieterId"" IS NOT NULL;
            ");

            // Migrate PlannedSessions.Location → HostMemberId
            // MUST happen BEFORE dropping Location column
            migrationBuilder.Sql(@"
                UPDATE ""PlannedSessions""
                SET ""HostMemberId"" = (SELECT ""Id"" FROM ""GroupMembers"" WHERE ""Nickname"" = LOWER(""Location""))
                WHERE ""Location"" IS NOT NULL AND ""Location"" != '';
            ");

            // Insert seed data that depends on Groups existing
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "BadgeColor", "Code", "GroupId", "Name", "SortOrder" },
                values: new object[] { 3, "s1", "S1", 2, "Schedule 1", 0 });

            migrationBuilder.InsertData(
                table: "NowPlaying",
                columns: new[] { "Id", "GameEntryId", "GroupId", "StatusNote", "UpdatedAt" },
                values: new object[] { 2, null, 2, null, new DateTime(2026, 3, 29, 0, 0, 0, 0, DateTimeKind.Utc) });

            // ──────────────────────────────────────────────────
            // Phase 4 — Enforce constraints + drop old columns
            // ──────────────────────────────────────────────────

            // Tighten GroupId to NOT NULL now that all rows are populated
            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "Categories",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "GameEntries",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "NowPlaying",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GroupId",
                table: "PlannedSessions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            // Drop old platform FKs and columns — data already migrated to MemberGamePlatforms
            migrationBuilder.DropForeignKey(
                name: "FK_GameEntries_Platforms_PlatformBrechtId",
                table: "GameEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_GameEntries_Platforms_PlatformPieterId",
                table: "GameEntries");

            migrationBuilder.DropIndex(
                name: "IX_GameEntries_PlatformBrechtId",
                table: "GameEntries");

            migrationBuilder.DropIndex(
                name: "IX_GameEntries_PlatformPieterId",
                table: "GameEntries");

            migrationBuilder.DropColumn(
                name: "PlatformBrechtId",
                table: "GameEntries");

            migrationBuilder.DropColumn(
                name: "PlatformPieterId",
                table: "GameEntries");

            // Drop Location — data already migrated to HostMemberId
            migrationBuilder.DropColumn(
                name: "Location",
                table: "PlannedSessions");

            // Add new indexes and FKs
            migrationBuilder.CreateIndex(
                name: "IX_Categories_GroupId",
                table: "Categories",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEntries_GroupId",
                table: "GameEntries",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_NowPlaying_GroupId",
                table: "NowPlaying",
                column: "GroupId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlannedSessions_GroupId",
                table: "PlannedSessions",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedSessions_HostMemberId",
                table: "PlannedSessions",
                column: "HostMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Groups_GroupId",
                table: "Categories",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameEntries_Groups_GroupId",
                table: "GameEntries",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NowPlaying_Groups_GroupId",
                table: "NowPlaying",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedSessions_GroupMembers_HostMemberId",
                table: "PlannedSessions",
                column: "HostMemberId",
                principalTable: "GroupMembers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PlannedSessions_Groups_GroupId",
                table: "PlannedSessions",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Groups_GroupId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_GameEntries_Groups_GroupId",
                table: "GameEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_NowPlaying_Groups_GroupId",
                table: "NowPlaying");

            migrationBuilder.DropForeignKey(
                name: "FK_PlannedSessions_GroupMembers_HostMemberId",
                table: "PlannedSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_PlannedSessions_Groups_GroupId",
                table: "PlannedSessions");

            migrationBuilder.DropTable(
                name: "MemberGamePlatforms");

            migrationBuilder.DropTable(
                name: "GroupMembers");

            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_PlannedSessions_GroupId",
                table: "PlannedSessions");

            migrationBuilder.DropIndex(
                name: "IX_PlannedSessions_HostMemberId",
                table: "PlannedSessions");

            migrationBuilder.DropIndex(
                name: "IX_NowPlaying_GroupId",
                table: "NowPlaying");

            migrationBuilder.DropIndex(
                name: "IX_GameEntries_GroupId",
                table: "GameEntries");

            migrationBuilder.DropIndex(
                name: "IX_Categories_GroupId",
                table: "Categories");

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "NowPlaying",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "PlannedSessions");

            migrationBuilder.DropColumn(
                name: "HostMemberId",
                table: "PlannedSessions");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "NowPlaying");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "GameEntries");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "Categories");

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "PlannedSessions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PlatformBrechtId",
                table: "GameEntries",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlatformPieterId",
                table: "GameEntries",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, 2 });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, 2 });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, 2 });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { 1, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { 1, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { 1, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 18,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 19,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 20,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 21,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, 2 });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 22,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 23,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { 3, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 24,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 25,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 26,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 27,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 28,
                columns: new[] { "PlatformBrechtId", "PlatformPieterId" },
                values: new object[] { null, null });

            migrationBuilder.CreateIndex(
                name: "IX_GameEntries_PlatformBrechtId",
                table: "GameEntries",
                column: "PlatformBrechtId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEntries_PlatformPieterId",
                table: "GameEntries",
                column: "PlatformPieterId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameEntries_Platforms_PlatformBrechtId",
                table: "GameEntries",
                column: "PlatformBrechtId",
                principalTable: "Platforms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameEntries_Platforms_PlatformPieterId",
                table: "GameEntries",
                column: "PlatformPieterId",
                principalTable: "Platforms",
                principalColumn: "Id");
        }
    }
}
