using Microsoft.EntityFrameworkCore.Migrations;

namespace DAO_DbService.Migrations
{
    public partial class jobpost_edited : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodeUrl",
                table: "JobPosts",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tags",
                table: "JobPosts",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CodeUrl",
                table: "JobPosts");

            migrationBuilder.DropColumn(
                name: "Tags",
                table: "JobPosts");
        }
    }
}
