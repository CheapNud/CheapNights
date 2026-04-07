using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheapNights.Migrations
{
    /// <inheritdoc />
    public partial class RestrictGroupOwnerCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_AppUsers_OwnerId",
                table: "Groups");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_AppUsers_OwnerId",
                table: "Groups",
                column: "OwnerId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_AppUsers_OwnerId",
                table: "Groups");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_AppUsers_OwnerId",
                table: "Groups",
                column: "OwnerId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
