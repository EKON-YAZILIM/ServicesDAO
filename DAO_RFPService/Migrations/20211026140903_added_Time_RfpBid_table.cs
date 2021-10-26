using Microsoft.EntityFrameworkCore.Migrations;

namespace DAO_RFPService.Migrations
{
    public partial class added_Time_RfpBid_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Time",
                table: "RfpBids",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "RfpBids");
        }
    }
}
