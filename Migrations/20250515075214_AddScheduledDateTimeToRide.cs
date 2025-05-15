using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetInfo.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduledDateTimeToRide : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledDateTime",
                table: "Rides",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduledDateTime",
                table: "Rides");
        }
    }
}
