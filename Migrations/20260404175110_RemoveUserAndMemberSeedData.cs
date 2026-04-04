using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheapNights.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserAndMemberSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Intentionally empty — seed data for AppUsers, GroupMembers, and Group.OwnerId
            // has been removed from the model. Production data is managed at runtime via
            // Plex auth and group management UI. The snapshot is updated to reflect this.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No rollback — seed data was placeholder only
        }
    }
}
