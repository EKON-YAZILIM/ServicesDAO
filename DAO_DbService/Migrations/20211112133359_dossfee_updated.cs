using Microsoft.EntityFrameworkCore.Migrations;

namespace DAO_DbService.Migrations
{
    public partial class dossfee_updated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DosPaid",
                table: "JobPosts");

            migrationBuilder.AddColumn<bool>(
                name: "DosFeePaid",
                table: "JobPosts",
                type: "tinyint(1)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DosFeePaid",
                table: "JobPosts");

            migrationBuilder.AddColumn<double>(
                name: "DosPaid",
                table: "JobPosts",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
