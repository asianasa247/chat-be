using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class RenameColumnWarehousePlanningProduceProductId : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "WarehousePlanningProduceProductId",
            table: "WarehouseProduceProductDetails",
            newName: "WarehouseProduceProductId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "WarehouseProduceProductId",
            table: "WarehouseProduceProductDetails",
            newName: "WarehousePlanningProduceProductId");
    }
}
