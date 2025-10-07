using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddNewColumnIntoTableLedgerWarehouse : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "LedgerInternalId",
            table: "LedgerFixedAssets");

        migrationBuilder.AddColumn<int>(
            name: "CustomerId",
            table: "LedgerWareHouses",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "IsInternal",
            table: "LedgerWareHouses",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "Month",
            table: "LedgerWareHouses",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "Type",
            table: "LedgerWareHouses",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CustomerId",
            table: "LedgerWareHouses");

        migrationBuilder.DropColumn(
            name: "IsInternal",
            table: "LedgerWareHouses");

        migrationBuilder.DropColumn(
            name: "Month",
            table: "LedgerWareHouses");

        migrationBuilder.DropColumn(
            name: "Type",
            table: "LedgerWareHouses");

        migrationBuilder.AddColumn<long>(
            name: "LedgerInternalId",
            table: "LedgerFixedAssets",
            type: "bigint",
            nullable: false,
            defaultValue: 0L);
    }
}
