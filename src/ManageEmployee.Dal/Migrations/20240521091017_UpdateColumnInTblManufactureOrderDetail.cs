using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateColumnInTblManufactureOrderDetail : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Quantity",
            table: "ManufactureOrderDetails",
            newName: "QuantityRequired");

        migrationBuilder.AddColumn<int>(
            name: "CarId",
            table: "ManufactureOrderDetails",
            type: "int",
            nullable: true);

        migrationBuilder.AddColumn<double>(
            name: "QuantityReal",
            table: "ManufactureOrderDetails",
            type: "float",
            nullable: false,
            defaultValue: 0.0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CarId",
            table: "ManufactureOrderDetails");

        migrationBuilder.DropColumn(
            name: "QuantityReal",
            table: "ManufactureOrderDetails");

        migrationBuilder.RenameColumn(
            name: "QuantityRequired",
            table: "ManufactureOrderDetails",
            newName: "Quantity");
    }
}
