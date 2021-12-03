using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace DAO_DbService.Migrations
{
    public partial class platformsetting_table_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlatformSettings",
                columns: table => new
                {
                    PlatformSettingID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserID = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    DosCurrencies = table.Column<string>(type: "text", nullable: true),
                    DosFees = table.Column<string>(type: "text", nullable: true),
                    DefaultPolicingRate = table.Column<double>(type: "double", nullable: true),
                    MinPolicingRate = table.Column<double>(type: "double", nullable: true),
                    MaxPolicingRate = table.Column<double>(type: "double", nullable: true),
                    ForumKYCRequired = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    QuorumRatio = table.Column<double>(type: "double", nullable: true),
                    InternalAuctionTime = table.Column<double>(type: "double", nullable: true),
                    PublicAuctionTime = table.Column<double>(type: "double", nullable: true),
                    AuctionTimeType = table.Column<string>(type: "text", nullable: true),
                    VotingTime = table.Column<double>(type: "double", nullable: true),
                    VotingTimeType = table.Column<string>(type: "text", nullable: true),
                    ReputationConversionRate = table.Column<double>(type: "double", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformSettings", x => x.PlatformSettingID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlatformSettings");
        }
    }
}
