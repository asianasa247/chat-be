using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddFieldAmountImportWarehouseIntoTableLedger : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<decimal>(
            name: "AmountImportWarehouse",
            table: "Ledgers",
            type: "decimal(18,2)",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "AmountImportWarehouse",
            table: "LedgerInternals",
            type: "decimal(18,2)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AmountImportWarehouse",
            table: "Ledgers");

        migrationBuilder.DropColumn(
            name: "AmountImportWarehouse",
            table: "LedgerInternals");
    }
}
