using Microsoft.EntityFrameworkCore.Migrations;

namespace TurnipTallyApi.Migrations
{
    public partial class UserTimezone : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimezoneId",
                table: "RegisteredUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimezoneId",
                table: "RegisteredUsers");
        }
    }
}
