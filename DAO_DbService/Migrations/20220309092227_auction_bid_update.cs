using Microsoft.EntityFrameworkCore.Migrations;

namespace DAO_DbService.Migrations
{
    public partial class auction_bid_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GithubLink",
                table: "AuctionBids",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Referrer",
                table: "AuctionBids",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResumeLink",
                table: "AuctionBids",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GithubLink",
                table: "AuctionBids");

            migrationBuilder.DropColumn(
                name: "Referrer",
                table: "AuctionBids");

            migrationBuilder.DropColumn(
                name: "ResumeLink",
                table: "AuctionBids");
        }
    }
}
