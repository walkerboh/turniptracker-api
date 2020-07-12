using Microsoft.EntityFrameworkCore.Migrations;

namespace TurnipTallyApi.Migrations
{
    public partial class timezonesOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Timezones",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Timezones");
        }
    }
}
