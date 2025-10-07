using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class UpdateColumnInvoiceInTblLedgerProduceExportDetails : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "InvoiceAdditionalDeclarationCode",
            table: "LedgerProduceImportDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "InvoiceAddress",
            table: "LedgerProduceImportDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "InvoiceCode",
            table: "LedgerProduceImportDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "InvoiceDate",
            table: "LedgerProduceImportDetails",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "InvoiceName",
            table: "LedgerProduceImportDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "InvoiceNumber",
            table: "LedgerProduceImportDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "InvoiceSerial",
            table: "LedgerProduceImportDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "InvoiceTaxCode",
            table: "LedgerProduceImportDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "InvoiceAdditionalDeclarationCode",
            table: "LedgerProduceExportDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "InvoiceAddress",
            table: "LedgerProduceExportDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "InvoiceCode",
            table: "LedgerProduceExportDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "InvoiceDate",
            table: "LedgerProduceExportDetails",
            type: "datetime2",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "InvoiceName",
            table: "LedgerProduceExportDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "InvoiceNumber",
            table: "LedgerProduceExportDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "InvoiceSerial",
            table: "LedgerProduceExportDetails",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "InvoiceTaxCode",
            table: "LedgerProduceExportDetails",
            type: "nvarchar(max)",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "InvoiceAdditionalDeclarationCode",
            table: "LedgerProduceImportDetails");

        migrationBuilder.DropColumn(
            name: "InvoiceAddress",
            table: "LedgerProduceImportDetails");

        migrationBuilder.DropColumn(
            name: "InvoiceCode",
            table: "LedgerProduceImportDetails");

        migrationBuilder.DropColumn(
            name: "InvoiceDate",
            table: "LedgerProduceImportDetails");

        migrationBuilder.DropColumn(
            name: "InvoiceName",
            table: "LedgerProduceImportDetails");

        migrationBuilder.DropColumn(
            name: "InvoiceNumber",
            table: "LedgerProduceImportDetails");

        migrationBuilder.DropColumn(
            name: "InvoiceSerial",
            table: "LedgerProduceImportDetails");

        migrationBuilder.DropColumn(
            name: "InvoiceTaxCode",
            table: "LedgerProduceImportDetails");

        migrationBuilder.DropColumn(
            name: "InvoiceAdditionalDeclarationCode",
            table: "LedgerProduceExportDetails");

        migrationBuilder.DropColumn(
            name: "InvoiceAddress",
            table: "LedgerProduceExportDetails");

        migrationBuilder.DropColumn(
            name: "InvoiceCode",
            table: "LedgerProduceExportDetails");

        migrationBuilder.DropColumn(
            name: "InvoiceDate",
            table: "LedgerProduceExportDetails");

        migrationBuilder.DropColumn(
            name: "InvoiceName",
            table: "LedgerProduceExportDetails");

        migrationBuilder.DropColumn(
            name: "InvoiceNumber",
            table: "LedgerProduceExportDetails");

        migrationBuilder.DropColumn(
            name: "InvoiceSerial",
            table: "LedgerProduceExportDetails");

        migrationBuilder.DropColumn(
            name: "InvoiceTaxCode",
            table: "LedgerProduceExportDetails");
    }
}
