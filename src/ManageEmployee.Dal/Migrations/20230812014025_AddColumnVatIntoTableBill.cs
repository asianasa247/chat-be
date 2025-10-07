using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddColumnVatIntoTableBill : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "Vat",
            table: "Bills",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);

        migrationBuilder.AddColumn<decimal>(
            name: "VatRate",
            table: "Bills",
            type: "decimal(18,2)",
            nullable: false,
            defaultValue: 0m);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Vat",
            table: "Bills");

        migrationBuilder.DropColumn(
            name: "VatRate",
            table: "Bills");
    }
}
