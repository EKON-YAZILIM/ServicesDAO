using Microsoft.EntityFrameworkCore.Migrations;

namespace DAO_DbService.Migrations
{
    public partial class flag_removed_from_jobs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsFlagged",
                table: "JobPosts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsFlagged",
                table: "JobPosts",
                type: "tinyint(1)",
                nullable: true);
        }
    }
}
