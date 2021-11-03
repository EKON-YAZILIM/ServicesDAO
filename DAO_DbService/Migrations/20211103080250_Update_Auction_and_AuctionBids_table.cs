using Microsoft.EntityFrameworkCore.Migrations;

namespace DAO_DbService.Migrations
{
    public partial class Update_Auction_and_AuctionBids_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInternal",
                table: "AuctionBids");

            migrationBuilder.AddColumn<bool>(
                name: "IsInternal",
                table: "Auctions",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsInternal",
                table: "Auctions");

            migrationBuilder.AddColumn<bool>(
                name: "IsInternal",
                table: "AuctionBids",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
