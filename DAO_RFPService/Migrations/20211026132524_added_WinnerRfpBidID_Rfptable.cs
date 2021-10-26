using Microsoft.EntityFrameworkCore.Migrations;

namespace DAO_RFPService.Migrations
{
    public partial class added_WinnerRfpBidID_Rfptable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WinnerRfpBidID",
                table: "Rfps",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WinnerRfpBidID",
                table: "Rfps");
        }
    }
}
