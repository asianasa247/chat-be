using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial95 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Account",
            table: "GoodWarehouseExport");

        migrationBuilder.DropColumn(
            name: "AccountName",
            table: "GoodWarehouseExport");

        migrationBuilder.DropColumn(
            name: "Detail1",
            table: "GoodWarehouseExport");

        migrationBuilder.RenameColumn(
            name: "DetailName2",
            table: "GoodWarehouseExport",
            newName: "QrCode");

        migrationBuilder.RenameColumn(
            name: "DetailName1",
            table: "GoodWarehouseExport",
            newName: "GoodsName");

        migrationBuilder.RenameColumn(
            name: "Detail2",
            table: "GoodWarehouseExport",
            newName: "GoodsCode");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "QrCode",
            table: "GoodWarehouseExport",
            newName: "DetailName2");

        migrationBuilder.RenameColumn(
            name: "GoodsName",
            table: "GoodWarehouseExport",
            newName: "DetailName1");

        migrationBuilder.RenameColumn(
            name: "GoodsCode",
            table: "GoodWarehouseExport",
            newName: "Detail2");

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

        migrationBuilder.AddColumn<string>(
            name: "Detail1",
            table: "GoodWarehouseExport",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);
    }
}
