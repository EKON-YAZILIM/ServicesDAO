using Microsoft.EntityFrameworkCore.Migrations;

namespace DAO_DbService.Migrations
{
    public partial class comment_pinned_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProgressType",
                table: "JobPosts");

            migrationBuilder.AddColumn<bool>(
                name: "IsPinned",
                table: "JobPostComments",
                type: "tinyint(1)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPinned",
                table: "JobPostComments");

            migrationBuilder.AddColumn<int>(
                name: "ProgressType",
                table: "JobPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
