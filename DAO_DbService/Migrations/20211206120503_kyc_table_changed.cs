using Microsoft.EntityFrameworkCore.Migrations;

namespace DAO_DbService.Migrations
{
    public partial class kyc_table_changed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicantId",
                table: "UserKYCs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "UserKYCs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileId1",
                table: "UserKYCs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileId2",
                table: "UserKYCs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KYCStatus",
                table: "UserKYCs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserID",
                table: "UserKYCs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VerificationId",
                table: "UserKYCs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Verified",
                table: "UserKYCs",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicantId",
                table: "UserKYCs");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "UserKYCs");

            migrationBuilder.DropColumn(
                name: "FileId1",
                table: "UserKYCs");

            migrationBuilder.DropColumn(
                name: "FileId2",
                table: "UserKYCs");

            migrationBuilder.DropColumn(
                name: "KYCStatus",
                table: "UserKYCs");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "UserKYCs");

            migrationBuilder.DropColumn(
                name: "VerificationId",
                table: "UserKYCs");

            migrationBuilder.DropColumn(
                name: "Verified",
                table: "UserKYCs");
        }
    }
}
