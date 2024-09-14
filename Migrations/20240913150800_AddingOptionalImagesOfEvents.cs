using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EventApp.Migrations
{
    /// <inheritdoc />
    public partial class AddingOptionalImagesOfEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OptionalImages",
                table: "Events");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OptionalImages",
                table: "Events",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
