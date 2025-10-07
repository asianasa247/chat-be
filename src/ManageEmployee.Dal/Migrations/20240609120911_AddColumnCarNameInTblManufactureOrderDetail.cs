using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnCarNameInTblManufactureOrderDetail : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "CreateAt",
            table: "ManufactureOrderDetails",
            newName: "CreatedAt");

        migrationBuilder.AddColumn<bool>(
            name: "IsProduced",
            table: "WarehouseProduceProductDetails",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<string>(
            name: "CarName",
            table: "ManufactureOrderDetails",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsProduced",
            table: "WarehouseProduceProductDetails");

        migrationBuilder.DropColumn(
            name: "CarName",
            table: "ManufactureOrderDetails");

        migrationBuilder.RenameColumn(
            name: "CreatedAt",
            table: "ManufactureOrderDetails",
            newName: "CreateAt");
    }
}
