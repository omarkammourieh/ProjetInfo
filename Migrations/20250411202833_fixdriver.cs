using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetInfo.Migrations
{
    /// <inheritdoc />
    public partial class fixdriver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DriverReviews");

            migrationBuilder.DropColumn(
                name: "CarModel",
                table: "Drivers");

            migrationBuilder.DropColumn(
                name: "CarPlate",
                table: "Drivers");

            migrationBuilder.AddColumn<int>(
                name: "DriverID",
                table: "RideFeedbacks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RideFeedbacks_DriverID",
                table: "RideFeedbacks",
                column: "DriverID");

            migrationBuilder.AddForeignKey(
                name: "FK_RideFeedbacks_Drivers_DriverID",
                table: "RideFeedbacks",
                column: "DriverID",
                principalTable: "Drivers",
                principalColumn: "DriverID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RideFeedbacks_Drivers_DriverID",
                table: "RideFeedbacks");

            migrationBuilder.DropIndex(
                name: "IX_RideFeedbacks_DriverID",
                table: "RideFeedbacks");

            migrationBuilder.DropColumn(
                name: "DriverID",
                table: "RideFeedbacks");

            migrationBuilder.AddColumn<string>(
                name: "CarModel",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CarPlate",
                table: "Drivers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DriverReviews",
                columns: table => new
                {
                    ReviewID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverReviews", x => x.ReviewID);
                    table.ForeignKey(
                        name: "FK_DriverReviews_Drivers_DriverID",
                        column: x => x.DriverID,
                        principalTable: "Drivers",
                        principalColumn: "DriverID");
                    table.ForeignKey(
                        name: "FK_DriverReviews_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DriverReviews_DriverID",
                table: "DriverReviews",
                column: "DriverID");

            migrationBuilder.CreateIndex(
                name: "IX_DriverReviews_UserID",
                table: "DriverReviews",
                column: "UserID");
        }
    }
}
