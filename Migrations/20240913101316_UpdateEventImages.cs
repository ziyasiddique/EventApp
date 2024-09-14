using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEventImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OptionalImagePath1",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OptionalImagePath2",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OptionalImagePath3",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OptionalImagePath4",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OptionalImagePath1",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "OptionalImagePath2",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "OptionalImagePath3",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "OptionalImagePath4",
                table: "Events");
        }
    }
}
