using Microsoft.EntityFrameworkCore.Migrations;

namespace DAO_LogService.Migrations
{
    public partial class dblogservice_name_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLog",
                table: "UserLog");

            migrationBuilder.RenameTable(
                name: "UserLog",
                newName: "UserLogs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLogs",
                table: "UserLogs",
                column: "UserLogId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserLogs",
                table: "UserLogs");

            migrationBuilder.RenameTable(
                name: "UserLogs",
                newName: "UserLog");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserLog",
                table: "UserLog",
                column: "UserLogId");
        }
    }
}
