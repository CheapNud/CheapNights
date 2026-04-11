using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheapNights.Migrations
{
    /// <inheritdoc />
    public partial class AddCalendarToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CalendarToken",
                table: "AppUsers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_CalendarToken",
                table: "AppUsers",
                column: "CalendarToken",
                unique: true,
                filter: "\"CalendarToken\" IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppUsers_CalendarToken",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "CalendarToken",
                table: "AppUsers");
        }
    }
}
