using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace DAO_DbService.Migrations
{
    public partial class Add_PaymentHistory_add_WalletAdd_Iban : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IBAN",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletAddress",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssociateUserNote",
                table: "AuctionBids",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentHistories",
                columns: table => new
                {
                    PaymentHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    JobID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<double>(type: "double", nullable: false),
                    WalletAddress = table.Column<string>(type: "text", nullable: true),
                    IBAN = table.Column<string>(type: "text", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentHistories", x => x.PaymentHistoryID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentHistories");

            migrationBuilder.DropColumn(
                name: "IBAN",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WalletAddress",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AssociateUserNote",
                table: "AuctionBids");
        }
    }
}
