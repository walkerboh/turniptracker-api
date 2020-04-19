using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TurnipTallyApi.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegisteredUsers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Email = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordSalt = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisteredUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Boards",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UrlName = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    Private = table.Column<bool>(nullable: false),
                    EditKey = table.Column<string>(nullable: true),
                    OwnerId = table.Column<long>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Boards_RegisteredUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "RegisteredUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BoardUsers",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    BoardId = table.Column<long>(nullable: false),
                    RegisteredUserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoardUsers_Boards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoardUsers_RegisteredUsers_RegisteredUserId",
                        column: x => x.RegisteredUserId,
                        principalTable: "RegisteredUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Weeks",
                columns: table => new
                {
                    BoardUserId = table.Column<long>(nullable: false),
                    WeekDate = table.Column<DateTime>(nullable: false),
                    BuyPrice = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Weeks", x => new { x.BoardUserId, x.WeekDate });
                    table.ForeignKey(
                        name: "FK_Weeks_BoardUsers_BoardUserId",
                        column: x => x.BoardUserId,
                        principalTable: "BoardUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Records",
                columns: table => new
                {
                    BoardUserId = table.Column<long>(nullable: false),
                    WeekDate = table.Column<DateTime>(nullable: false),
                    Day = table.Column<string>(nullable: false),
                    Period = table.Column<string>(nullable: false),
                    SellPrice = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Records", x => new { x.BoardUserId, x.WeekDate, x.Day, x.Period });
                    table.ForeignKey(
                        name: "FK_Records_Weeks_BoardUserId_WeekDate",
                        columns: x => new { x.BoardUserId, x.WeekDate },
                        principalTable: "Weeks",
                        principalColumns: new[] { "BoardUserId", "WeekDate" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Boards_OwnerId",
                table: "Boards",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_BoardUsers_BoardId",
                table: "BoardUsers",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_BoardUsers_RegisteredUserId",
                table: "BoardUsers",
                column: "RegisteredUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Records");

            migrationBuilder.DropTable(
                name: "Weeks");

            migrationBuilder.DropTable(
                name: "BoardUsers");

            migrationBuilder.DropTable(
                name: "Boards");

            migrationBuilder.DropTable(
                name: "RegisteredUsers");
        }
    }
}
