using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddParamTransportInLedger : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "FixedAsset242Id",
            table: "Ledgers",
            newName: "Tab");

        migrationBuilder.RenameColumn(
            name: "FixedAsset242Id",
            table: "LedgerInternals",
            newName: "Tab");

        migrationBuilder.AddColumn<decimal>(
            name: "AmountTransport",
            table: "Ledgers",
            type: "decimal(18,2)",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "PercentImportTax",
            table: "Ledgers",
            type: "decimal(18,2)",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "PercentTransport",
            table: "Ledgers",
            type: "decimal(18,2)",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "AmountTransport",
            table: "LedgerInternals",
            type: "decimal(18,2)",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "PercentImportTax",
            table: "LedgerInternals",
            type: "decimal(18,2)",
            nullable: true);

        migrationBuilder.AddColumn<decimal>(
            name: "PercentTransport",
            table: "LedgerInternals",
            type: "decimal(18,2)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "AmountTransport",
            table: "Ledgers");

        migrationBuilder.DropColumn(
            name: "PercentImportTax",
            table: "Ledgers");

        migrationBuilder.DropColumn(
            name: "PercentTransport",
            table: "Ledgers");

        migrationBuilder.DropColumn(
            name: "AmountTransport",
            table: "LedgerInternals");

        migrationBuilder.DropColumn(
            name: "PercentImportTax",
            table: "LedgerInternals");

        migrationBuilder.DropColumn(
            name: "PercentTransport",
            table: "LedgerInternals");

        migrationBuilder.RenameColumn(
            name: "Tab",
            table: "Ledgers",
            newName: "FixedAsset242Id");

        migrationBuilder.RenameColumn(
            name: "Tab",
            table: "LedgerInternals",
            newName: "FixedAsset242Id");
    }
}
