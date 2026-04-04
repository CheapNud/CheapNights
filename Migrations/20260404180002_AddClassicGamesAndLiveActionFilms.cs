using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CheapNights.Migrations
{
    /// <inheritdoc />
    public partial class AddClassicGamesAndLiveActionFilms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GroupMembers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AppUsers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.InsertData(
                table: "GameEntries",
                columns: new[] { "Id", "CategoryId", "CompletedAt", "CompletedTime", "EntryTypeId", "GameNote", "GroupId", "IsCompleted", "IsCouchCoop", "IsMovie", "LengthLabel", "Name", "Protagonist", "SortLabel", "SortOrder", "StarRating", "StatusId", "StoryEra" },
                values: new object[,]
                {
                    { 29, 1, null, null, 1, "HD remaster of the 2002 GameCube remake — the definitive version of the original Spencer Mansion story until the new remake lands", 1, false, false, false, "10h", "RE HD REMASTER (2015)", "Chris / Jill", "7", 20, 2, 2, "1998" },
                    { 30, 1, null, null, 1, "Prequel — Rebecca & Billy on the Ecliptic Express the night before RE1. Adds context but not essential to the main arc", 1, false, false, false, "12h", "RE0 HD REMASTER", "Rebecca + Billy", "7b", 21, 1, 3, "1998" },
                    { 31, 1, null, null, 1, "The original that redefined the genre — play if you want to compare with the remake or experience the campier tone", 1, false, false, false, "16h", "RE4 ORIGINAL (2005)", "Leon S. Kennedy", "—", 22, 3, 3, "2004" },
                    { 32, 1, null, null, 2, "The Hive, T-virus outbreak, Red Queen — started the film franchise. Loosely inspired by the games", 1, false, false, true, "1.5h", "RESIDENT EVIL (2002)", "Alice (Milla Jovovich)", null, 30, 2, 3, "2002" },
                    { 33, 1, null, null, 2, "Raccoon City outbreak with Nemesis — introduces Jill and Carlos from the games", 1, false, false, true, "1.5h", "RE: APOCALYPSE (2004)", "Alice + Jill Valentine", null, 31, 1, 3, "2004" },
                    { 34, 1, null, null, 2, "Post-apocalyptic desert wasteland — Alice convoy vs Umbrella. Claire Redfield joins", 1, false, false, true, "1.5h", "RE: EXTINCTION (2007)", "Alice", null, 32, 1, 3, "2007" },
                    { 35, 1, null, null, 2, "Los Angeles prison siege — Wesker showdown. First in 3D", 1, false, false, true, "1.5h", "RE: AFTERLIFE (2010)", "Alice + Claire + Chris", null, 33, 1, 3, "2010" },
                    { 36, 1, null, null, 2, "Umbrella simulation facility — Ada Wong and Leon join. Most game-character-heavy entry", 1, false, false, true, "1.5h", "RE: RETRIBUTION (2012)", "Alice + Ada + Leon", null, 34, 1, 3, "2012" },
                    { 37, 1, null, null, 2, "Return to the Hive — wraps up the Anderson/Jovovich saga", 1, false, false, true, "1.5h", "RE: THE FINAL CHAPTER (2016)", "Alice", null, 35, 1, 3, "2016" },
                    { 38, 1, null, null, 2, "Faithful reboot combining RE1 + RE2 — Spencer Mansion and RPD in one night. Closer to the games than the Anderson films", 1, false, false, true, "1.5h", "RE: WELCOME TO RACCOON CITY (2021)", "Claire + Leon + Chris", null, 36, 2, 3, "2021" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "GameEntries",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "AvatarUrl", "CreatedAt", "DisplayName", "PlexUserId" },
                values: new object[] { 3, null, new DateTime(2026, 3, 29, 0, 0, 0, 0, DateTimeKind.Utc), "Miel", "miel" });

            migrationBuilder.InsertData(
                table: "GroupMembers",
                columns: new[] { "Id", "AppUserId", "GroupId", "Nickname" },
                values: new object[] { 4, 3, 2, "miel" });
        }
    }
}
