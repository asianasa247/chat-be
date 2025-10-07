using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial98 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "WareHouseCode",
            table: "P_Inventories",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "QuantityInput",
            table: "GoodWarehouses",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "WareHouseCode",
            table: "P_Inventories");

        migrationBuilder.DropColumn(
            name: "QuantityInput",
            table: "GoodWarehouses");
    }
}
