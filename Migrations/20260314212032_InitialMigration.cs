using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CheapNights.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    BadgeColor = table.Column<string>(type: "TEXT", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntryTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Platforms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    ChipColor = table.Column<string>(type: "TEXT", nullable: true),
                    ChipBackground = table.Column<string>(type: "TEXT", nullable: true),
                    StripeColor = table.Column<string>(type: "TEXT", nullable: true),
                    IsSelectable = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GameEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CategoryId = table.Column<int>(type: "INTEGER", nullable: false),
                    EntryTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Protagonist = table.Column<string>(type: "TEXT", nullable: true),
                    StoryEra = table.Column<string>(type: "TEXT", nullable: true),
                    StatusId = table.Column<int>(type: "INTEGER", nullable: true),
                    StarRating = table.Column<int>(type: "INTEGER", nullable: false),
                    LengthLabel = table.Column<string>(type: "TEXT", nullable: true),
                    PlatformBrechtId = table.Column<int>(type: "INTEGER", nullable: true),
                    PlatformPieterId = table.Column<int>(type: "INTEGER", nullable: true),
                    IsCouchCoop = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsMovie = table.Column<bool>(type: "INTEGER", nullable: false),
                    SortLabel = table.Column<string>(type: "TEXT", nullable: true),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedTime = table.Column<string>(type: "TEXT", nullable: true),
                    GameNote = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameEntries_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameEntries_EntryTypes_EntryTypeId",
                        column: x => x.EntryTypeId,
                        principalTable: "EntryTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameEntries_Platforms_PlatformBrechtId",
                        column: x => x.PlatformBrechtId,
                        principalTable: "Platforms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GameEntries_Platforms_PlatformPieterId",
                        column: x => x.PlatformPieterId,
                        principalTable: "Platforms",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GameEntries_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NowPlaying",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameEntryId = table.Column<int>(type: "INTEGER", nullable: true),
                    StatusNote = table.Column<string>(type: "TEXT", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NowPlaying", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NowPlaying_GameEntries_GameEntryId",
                        column: x => x.GameEntryId,
                        principalTable: "GameEntries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PlannedSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScheduledAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    GameEntryId = table.Column<int>(type: "INTEGER", nullable: true),
                    CustomGame = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    UseCurrentGame = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlannedSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlannedSessions_GameEntries_GameEntryId",
                        column: x => x.GameEntryId,
                        principalTable: "GameEntries",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "BadgeColor", "Code", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "re", "RE", "Resident Evil", 0 },
                    { 2, "sh", "SH", "Silent Hill", 1 }
                });

            migrationBuilder.InsertData(
                table: "EntryTypes",
                columns: new[] { "Id", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "Game", 0 },
                    { 2, "Movie", 1 },
                    { 3, "DLC", 2 }
                });

            migrationBuilder.InsertData(
                table: "Platforms",
                columns: new[] { "Id", "Name", "SortOrder" },
                values: new object[,]
                {
                    { 1, "Steam", 0 },
                    { 2, "PS5", 1 },
                    { 3, "Plex", 2 }
                });

            migrationBuilder.InsertData(
                table: "Statuses",
                columns: new[] { "Id", "ChipBackground", "ChipColor", "IsSelectable", "Name", "SortOrder", "StripeColor" },
                values: new object[,]
                {
                    { 1, "rgba(192,57,43,0.1)", "#e05a4a", true, "Essential", 0, "var(--re-red)" },
                    { 2, "rgba(150,150,150,0.08)", "#888", true, "Recommended", 1, "#888" },
                    { 3, "rgba(110,110,110,0.07)", "#666", true, "Optional", 2, null },
                    { 4, null, "#444", true, "Skip", 3, "var(--skip-grey)" },
                    { 5, "rgba(124,92,191,0.1)", "#a98be8", true, "Upcoming", 4, "var(--upcoming)" }
                });

            migrationBuilder.InsertData(
                table: "GameEntries",
                columns: new[] { "Id", "CategoryId", "CompletedAt", "CompletedTime", "EntryTypeId", "GameNote", "IsCompleted", "IsCouchCoop", "IsMovie", "LengthLabel", "Name", "PlatformBrechtId", "PlatformPieterId", "Protagonist", "SortLabel", "SortOrder", "StarRating", "StatusId", "StoryEra" },
                values: new object[,]
                {
                    { 1, 1, null, null, 1, "Grace & Leon face a new Megamycete threat — series finale, wraps up the Winters saga", true, false, false, "15h", "RE9 REQUIEM", null, 2, "Grace + Leon", "✓", 0, 3, 1, "2026" },
                    { 2, 1, null, null, 1, "Ethan searches for Mia in the Baker estate — first-person horror, soft reboot of the series", false, false, false, "9h", "RE7 BIOHAZARD", null, 2, "Ethan Winters", "1", 1, 2, 1, "2017" },
                    { 3, 1, null, null, 1, "Play immediately after RE7 — direct sequel", false, false, false, "10h", "RE8 VILLAGE", null, 2, "Ethan Winters", "2", 2, 2, 1, "2021" },
                    { 4, 1, null, null, 2, "Leon, Chris, Jill, Claire & Rebecca reunite — set after Village", false, false, true, "1.5h", "RE: DEATH ISLAND (2023)", null, null, "Animated (CG)", null, 3, 1, 2, "2015" },
                    { 5, 1, null, null, 1, "Closes the Wesker arc", false, true, false, "12h", "RE5", 1, null, "Chris + Sheva", "3", 4, 2, 1, "2009" },
                    { 6, 1, null, null, 2, "Leon in a bio-war zone — bridges RE5 and RE6", false, false, true, "1.5h", "RE: DAMNATION (2012)", null, null, "Animated (CG)", null, 5, 1, 2, "2011" },
                    { 7, 1, null, null, 2, "Leon + Chris + Rebecca, set after RE6 — watch after RE6 recap", false, false, true, "1.5h", "RE: VENDETTA (2017)", null, null, "Animated (CG)", null, 6, 1, 2, "2014" },
                    { 8, 1, null, null, 1, "Jill + Chris on a cruise ship — fills the RE5→RE4 gap", false, false, false, "12h", "RE: REVELATIONS", null, null, "Jill Valentine", "3b", 7, 2, 2, "2005" },
                    { 9, 1, null, null, 1, "Leon at his best", false, false, false, "16h", "RE4 REMAKE", null, null, "Leon S. Kennedy", "4", 8, 3, 1, "2004" },
                    { 10, 1, null, null, 3, "Ada Wong's parallel story — recontextualizes RE4R. Play immediately after.", false, false, false, "6h", "RE4R: SEPARATE WAYS", null, null, "Ada Wong", "4+", 9, 1, 1, "2004" },
                    { 11, 1, null, null, 2, "Leon + Claire reunite ~1yr after RE4", false, false, true, "1.5h", "RE: DEGENERATION (2008)", null, null, "Animated (CG)", null, 10, 1, 2, "2005" },
                    { 12, 1, null, null, 2, "Netflix CG series — Leon & Claire, White House conspiracy", false, false, true, "2h", "RE: INFINITE DARKNESS (2021)", null, null, "Netflix (CG)", null, 11, 1, 2, "2006" },
                    { 13, 1, null, null, 1, "Claire + Barry, prison camp horror — has co-op!", false, true, false, "12h", "RE: REVELATIONS 2", null, null, "Claire + Barry", "5b", 12, 2, 2, "2011" },
                    { 14, 1, null, null, 1, "Raccoon City's final days", false, false, false, "6h", "RE3 REMAKE", 1, null, "Jill Valentine", "5", 13, 1, 2, "1998" },
                    { 15, 1, null, null, 1, "Leon's origin. Where it all began for him.", false, false, false, "8h", "RE2 REMAKE", null, null, "Leon + Claire", "6", 14, 2, 1, "1998" },
                    { 16, 1, null, null, 1, "Watch a story recap instead", false, false, false, "21h", "RE6", null, null, "Leon / Chris / Jake", "—", 15, 3, 4, "2013" },
                    { 17, 1, null, null, 1, "Non-canon multiplayer, dead servers", false, false, false, null, "RE RESISTANCE", 1, null, "Various", "—", 16, 0, 4, "" },
                    { 18, 1, null, null, 1, "Rumored 2027 — Claire's story", false, false, false, null, "CODE VERONICA REMAKE", null, null, "Claire Redfield", "🔜", 17, 0, 5, "2000" },
                    { 19, 1, null, null, 2, "Dir. Zach Cregger (Barbarian)", false, false, true, "2h", "RESIDENT EVIL (2026)", null, null, "Live-action reboot", "🔜", 18, 2, 5, "2026" },
                    { 20, 1, null, null, 1, "Early dev — 4–7 years out", false, false, false, null, "RE1 REMAKE", null, null, "Chris / Jill", "🔜", 19, 0, 5, "1998" },
                    { 21, 2, null, null, 1, "FREE on PS5 — standalone intro to Silent Hill themes, modern setting with a new protagonist", false, false, false, "2h", "SH: THE SHORT MESSAGE", null, 2, "New protagonist", "0", 100, 1, 1, "2024" },
                    { 22, 2, null, null, 1, "James Sunderland returns to Silent Hill after receiving a letter from his dead wife — psychological horror masterpiece", false, false, false, "10h", "SILENT HILL 2 REMAKE", null, null, "Psychological grief horror", "1", 101, 2, 1, "2024" },
                    { 23, 2, null, null, 2, "Live-action adaptation of SH2 — directed by Christophe Gans, compare with the game version", false, false, true, "2h", "RETURN TO SILENT HILL (2026)", 3, null, "Live-action", null, 102, 2, 2, "2026" },
                    { 24, 2, null, null, 2, "Own continuity loosely adapting SH1 + SH3 — not canon to games, optional horror movie night", false, false, true, "4h", "SILENT HILL (2006) + REVELATION (2012)", null, null, "Live-action duo", null, 103, 2, 3, "2006" },
                    { 25, 2, null, null, 1, "Set in 1960s Japan with a completely new story — bold departure from Americana fog, kowloon-style horror", false, false, false, "15h", "SILENT HILL f", null, null, "1960s Japan, new setting", "2", 104, 3, 1, "2025" },
                    { 26, 2, null, null, 1, "Alex Shepherd returns to Shepherd's Glen — combat-focused, Western-developed, widely considered weakest entry", false, false, false, null, "SH HOMECOMING", null, null, "Action-heavy, less atmospheric", "—", 105, 0, 4, "2008" },
                    { 27, 2, null, null, 1, "By No Code (Stories Untold) — Scotland 1996, first-person, P.T.-inspired indie horror set in the SH universe", false, false, false, null, "SH: TOWNFALL", null, null, "PT-inspired indie horror", "🔜", 106, 0, 5, "2026" },
                    { 28, 2, null, null, 1, "Bloober Team remaking the original — Harry Mason searching for his daughter Cheryl, the story that started it all", false, false, false, null, "SH1 REMAKE", null, null, "Original Spencer Mansion", "🔜", 107, 0, 5, "2027" }
                });

            migrationBuilder.InsertData(
                table: "NowPlaying",
                columns: new[] { "Id", "GameEntryId", "StatusNote", "UpdatedAt" },
                values: new object[] { 1, 1, "almost finished, Arc reached", new DateTime(2026, 3, 13, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.CreateIndex(
                name: "IX_GameEntries_CategoryId",
                table: "GameEntries",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEntries_EntryTypeId",
                table: "GameEntries",
                column: "EntryTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEntries_PlatformBrechtId",
                table: "GameEntries",
                column: "PlatformBrechtId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEntries_PlatformPieterId",
                table: "GameEntries",
                column: "PlatformPieterId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEntries_StatusId",
                table: "GameEntries",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_NowPlaying_GameEntryId",
                table: "NowPlaying",
                column: "GameEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_PlannedSessions_GameEntryId",
                table: "PlannedSessions",
                column: "GameEntryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NowPlaying");

            migrationBuilder.DropTable(
                name: "PlannedSessions");

            migrationBuilder.DropTable(
                name: "GameEntries");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "EntryTypes");

            migrationBuilder.DropTable(
                name: "Platforms");

            migrationBuilder.DropTable(
                name: "Statuses");
        }
    }
}
