using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnQuantityRequiredIntoTblWarehouseProduceProductDetail : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Quantity",
            table: "WarehouseProduceProductDetails",
            newName: "QuantityRequired");

        migrationBuilder.RenameColumn(
            name: "WarehousePlanningProduceProductId",
            table: "ManufactureOrderDetails",
            newName: "WarehousePlanningProduceProductDetailId");

        migrationBuilder.AlterColumn<int>(
            name: "CarId",
            table: "WarehouseProduceProductDetails",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");

        migrationBuilder.AddColumn<double>(
            name: "QuantityReal",
            table: "WarehouseProduceProductDetails",
            type: "float",
            nullable: false,
            defaultValue: 0.0);

        migrationBuilder.AddColumn<int>(
            name: "CustomerId",
            table: "ManufactureOrderDetails",
            type: "int",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "QuantityReal",
            table: "WarehouseProduceProductDetails");

        migrationBuilder.DropColumn(
            name: "CustomerId",
            table: "ManufactureOrderDetails");

        migrationBuilder.RenameColumn(
            name: "QuantityRequired",
            table: "WarehouseProduceProductDetails",
            newName: "Quantity");

        migrationBuilder.RenameColumn(
            name: "WarehousePlanningProduceProductDetailId",
            table: "ManufactureOrderDetails",
            newName: "WarehousePlanningProduceProductId");

        migrationBuilder.AlterColumn<int>(
            name: "CarId",
            table: "WarehouseProduceProductDetails",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);
    }
}
