using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial99 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Account",
            table: "P_Inventory_Items");

        migrationBuilder.DropColumn(
            name: "AccountName",
            table: "P_Inventory_Items");

        migrationBuilder.DropColumn(
            name: "DateExpiration",
            table: "P_Inventory_Items");

        migrationBuilder.DropColumn(
            name: "Detail1",
            table: "P_Inventory_Items");

        migrationBuilder.DropColumn(
            name: "Detail2",
            table: "P_Inventory_Items");

        migrationBuilder.DropColumn(
            name: "DetailName1",
            table: "P_Inventory_Items");

        migrationBuilder.DropColumn(
            name: "DetailName2",
            table: "P_Inventory_Items");

        migrationBuilder.DropColumn(
            name: "Warehouse",
            table: "P_Inventory_Items");

        migrationBuilder.DropColumn(
            name: "WarehouseName",
            table: "P_Inventory_Items");

        migrationBuilder.RenameColumn(
            name: "WareHouseCode",
            table: "P_Inventories",
            newName: "Warehouse");

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "P_Inventory_Items",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "GoodsCode",
            table: "P_Inventory_Items",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "GoodsName",
            table: "P_Inventory_Items",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "QrCode",
            table: "P_Inventory_Items",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Detail1",
            table: "P_Inventories",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Detail2",
            table: "P_Inventories",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DetailName1",
            table: "P_Inventories",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DetailName2",
            table: "P_Inventories",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "WarehouseName",
            table: "P_Inventories",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "GoodsCode",
            table: "P_Inventory_Items");

        migrationBuilder.DropColumn(
            name: "GoodsName",
            table: "P_Inventory_Items");

        migrationBuilder.DropColumn(
            name: "QrCode",
            table: "P_Inventory_Items");

        migrationBuilder.DropColumn(
            name: "Detail1",
            table: "P_Inventories");

        migrationBuilder.DropColumn(
            name: "Detail2",
            table: "P_Inventories");

        migrationBuilder.DropColumn(
            name: "DetailName1",
            table: "P_Inventories");

        migrationBuilder.DropColumn(
            name: "DetailName2",
            table: "P_Inventories");

        migrationBuilder.DropColumn(
            name: "WarehouseName",
            table: "P_Inventories");

        migrationBuilder.RenameColumn(
            name: "Warehouse",
            table: "P_Inventories",
            newName: "WareHouseCode");

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "P_Inventory_Items",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(500)",
            oldMaxLength: 500,
            oldNullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Account",
            table: "P_Inventory_Items",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "AccountName",
            table: "P_Inventory_Items",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "DateExpiration",
            table: "P_Inventory_Items",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Detail1",
            table: "P_Inventory_Items",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Detail2",
            table: "P_Inventory_Items",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DetailName1",
            table: "P_Inventory_Items",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "DetailName2",
            table: "P_Inventory_Items",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Warehouse",
            table: "P_Inventory_Items",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "WarehouseName",
            table: "P_Inventory_Items",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);
    }
}
