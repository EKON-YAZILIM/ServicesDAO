using Microsoft.EntityFrameworkCore.Migrations;

namespace DAO_DbService.Migrations
{
    public partial class auction_edited : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DAOMemberCount",
                table: "Auctions");

            migrationBuilder.AddColumn<double>(
                name: "ReputationStake",
                table: "AuctionBids",
                type: "double",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReputationStake",
                table: "AuctionBids");

            migrationBuilder.AddColumn<int>(
                name: "DAOMemberCount",
                table: "Auctions",
                type: "int",
                nullable: true);
        }
    }
}
