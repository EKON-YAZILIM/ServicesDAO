using Microsoft.EntityFrameworkCore.Migrations;

namespace DAO_DbService.Migrations
{
    public partial class governance_payment_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "GovernancePaymentRatio",
                table: "PlatformSettings",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GovernanceWallet",
                table: "PlatformSettings",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GovernancePaymentRatio",
                table: "PlatformSettings");

            migrationBuilder.DropColumn(
                name: "GovernanceWallet",
                table: "PlatformSettings");
        }
    }
}
