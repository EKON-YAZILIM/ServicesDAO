using System;
using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

namespace DAO_ReputationService.Migrations
{
    public partial class reputation_models_adjustment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reputations");

            migrationBuilder.CreateTable(
                name: "UserReputationHistories",
                columns: table => new
                {
                    UserReputationHistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Input = table.Column<double>(type: "double", nullable: false),
                    Output = table.Column<double>(type: "double", nullable: false),
                    LastTotal = table.Column<double>(type: "double", nullable: false),
                    Explanation = table.Column<string>(type: "text", nullable: true),
                    UserReputationID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReputationHistories", x => x.UserReputationHistoryID);
                });

            migrationBuilder.CreateTable(
                name: "UserReputations",
                columns: table => new
                {
                    UserReputationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Reputation = table.Column<double>(type: "double", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserReputations", x => x.UserReputationID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserReputationHistories");

            migrationBuilder.DropTable(
                name: "UserReputations");

            migrationBuilder.CreateTable(
                name: "Reputations",
                columns: table => new
                {
                    itemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reputations", x => x.itemId);
                });
        }
    }
}
