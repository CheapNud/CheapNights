using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheapNights.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRuntimeSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // No-op — runtime data (Groups, Categories, GameEntries, NowPlaying,
            // MemberGamePlatforms, AppUsers, GroupMembers) removed from HasData.
            // Production rows are managed through the UI and stay untouched.
            // Snapshot updated to stop EF from tracking these as seed data.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
