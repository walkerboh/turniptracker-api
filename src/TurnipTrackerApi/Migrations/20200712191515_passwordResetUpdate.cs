using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TurnipTallyApi.Migrations
{
    public partial class passwordResetUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PasswordResets",
                table: "PasswordResets");

            migrationBuilder.AlterColumn<long>(
                name: "RegisteredUserId",
                table: "PasswordResets",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PasswordResets",
                table: "PasswordResets",
                column: "Key");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PasswordResets",
                table: "PasswordResets");

            migrationBuilder.AlterColumn<long>(
                name: "RegisteredUserId",
                table: "PasswordResets",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long))
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PasswordResets",
                table: "PasswordResets",
                column: "RegisteredUserId");
        }
    }
}
