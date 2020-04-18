using Microsoft.EntityFrameworkCore.Migrations;

namespace TurnipTrackerApi.Migrations
{
    public partial class BoardUserDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "BoardUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "BoardUsers");
        }
    }
}
