using Microsoft.EntityFrameworkCore.Migrations;

namespace DAO_DbService.Migrations
{
    public partial class platform_settings_simplevote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "SimpleVotingTime",
                table: "PlatformSettings",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SimpleVotingTimeType",
                table: "PlatformSettings",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SimpleVotingTime",
                table: "PlatformSettings");

            migrationBuilder.DropColumn(
                name: "SimpleVotingTimeType",
                table: "PlatformSettings");
        }
    }
}
