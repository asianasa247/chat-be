using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnIsCanceledIntoTblManufactureOrders : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsCanceled",
            table: "ManufactureOrders",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<string>(
            name: "Note",
            table: "ManufactureOrderDetails",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsCanceled",
            table: "ManufactureOrders");

        migrationBuilder.DropColumn(
            name: "Note",
            table: "ManufactureOrderDetails");
    }
}
