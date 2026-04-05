using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheapNights.Migrations
{
    /// <inheritdoc />
    public partial class RequireGroupOwner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Assign any ownerless groups to their first member before enforcing NOT NULL
            migrationBuilder.Sql(@"
                UPDATE ""Groups"" g
                SET ""OwnerId"" = (
                    SELECT ""AppUserId"" FROM ""GroupMembers""
                    WHERE ""GroupId"" = g.""Id""
                    ORDER BY ""Id"" LIMIT 1
                )
                WHERE ""OwnerId"" IS NULL;
            ");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_AppUsers_OwnerId",
                table: "Groups");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Groups",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_AppUsers_OwnerId",
                table: "Groups",
                column: "OwnerId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_AppUsers_OwnerId",
                table: "Groups");

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Groups",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_AppUsers_OwnerId",
                table: "Groups",
                column: "OwnerId",
                principalTable: "AppUsers",
                principalColumn: "Id");
        }
    }
}
