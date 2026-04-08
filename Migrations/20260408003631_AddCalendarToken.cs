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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CalendarToken",
                table: "AppUsers");
        }
    }
}
