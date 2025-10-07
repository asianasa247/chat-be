using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class changeHistorySpin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Account",
                table: "HistorySpins");

            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "HistorySpins");

            migrationBuilder.DropColumn(
                name: "Detail1",
                table: "HistorySpins");

            migrationBuilder.DropColumn(
                name: "Detail2",
                table: "HistorySpins");

            migrationBuilder.DropColumn(
                name: "DetailName1",
                table: "HistorySpins");

            migrationBuilder.DropColumn(
                name: "DetailName2",
                table: "HistorySpins");

            migrationBuilder.DropColumn(
                name: "Warehouse",
                table: "HistorySpins");

            migrationBuilder.DropColumn(
                name: "WarehouseName",
                table: "HistorySpins");

            migrationBuilder.AddColumn<int>(
                name: "GoodId",
                table: "HistorySpins",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoodId",
                table: "HistorySpins");

            migrationBuilder.AddColumn<string>(
                name: "Account",
                table: "HistorySpins",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "HistorySpins",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Detail1",
                table: "HistorySpins",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Detail2",
                table: "HistorySpins",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DetailName1",
                table: "HistorySpins",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DetailName2",
                table: "HistorySpins",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Warehouse",
                table: "HistorySpins",
                type: "nvarchar(36)",
                maxLength: 36,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WarehouseName",
                table: "HistorySpins",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
