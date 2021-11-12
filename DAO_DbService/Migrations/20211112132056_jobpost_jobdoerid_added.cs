using Microsoft.EntityFrameworkCore.Migrations;

namespace DAO_DbService.Migrations
{
    public partial class jobpost_jobdoerid_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "JobDoerUserID",
                table: "JobPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JobDoerUserID",
                table: "JobPosts");
        }
    }
}
