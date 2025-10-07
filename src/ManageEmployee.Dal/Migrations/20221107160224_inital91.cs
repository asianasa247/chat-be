using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class inital91 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Account",
            table: "GoodWarehouseExport",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "AccountName",
            table: "GoodWarehouseExport",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "DateExpiration",
            table: "GoodWarehouseExport",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Detail1",
            table: "GoodWarehouseExport",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Detail2",
            table: "GoodWarehouseExport",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DetailName1",
            table: "GoodWarehouseExport",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DetailName2",
            table: "GoodWarehouseExport",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Warehouse",
            table: "GoodWarehouseExport",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "WarehouseName",
            table: "GoodWarehouseExport",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Account",
            table: "GoodWarehouseExport");

        migrationBuilder.DropColumn(
            name: "AccountName",
            table: "GoodWarehouseExport");

        migrationBuilder.DropColumn(
            name: "DateExpiration",
            table: "GoodWarehouseExport");

        migrationBuilder.DropColumn(
            name: "Detail1",
            table: "GoodWarehouseExport");

        migrationBuilder.DropColumn(
            name: "Detail2",
            table: "GoodWarehouseExport");

        migrationBuilder.DropColumn(
            name: "DetailName1",
            table: "GoodWarehouseExport");

        migrationBuilder.DropColumn(
            name: "DetailName2",
            table: "GoodWarehouseExport");

        migrationBuilder.DropColumn(
            name: "Warehouse",
            table: "GoodWarehouseExport");

        migrationBuilder.DropColumn(
            name: "WarehouseName",
            table: "GoodWarehouseExport");
    }
}
