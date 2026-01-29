using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeSpot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCityCoordinates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "latitude",
                table: "cities",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "longitude",
                table: "cities",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "latitude",
                table: "cities");

            migrationBuilder.DropColumn(
                name: "longitude",
                table: "cities");
        }
    }
}
