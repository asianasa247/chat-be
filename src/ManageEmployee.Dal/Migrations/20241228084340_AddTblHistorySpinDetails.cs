using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class AddTblHistorySpinDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "HistorySpins");

            migrationBuilder.DropColumn(
                name: "GoodId",
                table: "HistorySpins");

            migrationBuilder.CreateTable(
                name: "HistorySpinDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HistorySpinId = table.Column<int>(type: "int", nullable: false),
                    SettingsSpinId = table.Column<int>(type: "int", nullable: false),
                    PrizeId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    GoodId = table.Column<int>(type: "int", nullable: false),
                    WinTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceivedDay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistorySpinDetails", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistorySpinDetails");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "HistorySpins",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoodId",
                table: "HistorySpins",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
