using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial100 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "CloseQuantityReal",
            table: "P_Inventory_Items",
            newName: "QuantityReal");

        migrationBuilder.RenameColumn(
            name: "CloseQuantity",
            table: "P_Inventory_Items",
            newName: "Quantity");

        migrationBuilder.RenameColumn(
            name: "Value",
            table: "MenuKpis",
            newName: "Point");

        migrationBuilder.RenameColumn(
            name: "Sales",
            table: "MenuKpis",
            newName: "ToValue");

        migrationBuilder.AddColumn<decimal>(
            name: "FromValue",
            table: "MenuKpis",
            type: "decimal(18,2)",
            nullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "CustomerQuote_Detail",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "FromValue",
            table: "MenuKpis");

        migrationBuilder.RenameColumn(
            name: "QuantityReal",
            table: "P_Inventory_Items",
            newName: "CloseQuantityReal");

        migrationBuilder.RenameColumn(
            name: "Quantity",
            table: "P_Inventory_Items",
            newName: "CloseQuantity");

        migrationBuilder.RenameColumn(
            name: "ToValue",
            table: "MenuKpis",
            newName: "Sales");

        migrationBuilder.RenameColumn(
            name: "Point",
            table: "MenuKpis",
            newName: "Value");

        migrationBuilder.AlterColumn<string>(
            name: "Note",
            table: "CustomerQuote_Detail",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);
    }
}
