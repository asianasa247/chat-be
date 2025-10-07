using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddQuantityInWareHouse : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Quatity",
            table: "GoodWarehousesPositions",
            newName: "Quantity");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Quantity",
            table: "GoodWarehousesPositions",
            newName: "Quatity");
    }
}
