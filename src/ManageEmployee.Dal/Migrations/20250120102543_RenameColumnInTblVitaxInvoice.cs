using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations
{
    public partial class RenameColumnInTblVitaxInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "VintaxInvoiceIns",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "tgtttbso",
                table: "VintaxInvoiceIns",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "shdon",
                table: "VintaxInvoiceIns",
                newName: "SellerTax");

            migrationBuilder.RenameColumn(
                name: "ntao",
                table: "VintaxInvoiceIns",
                newName: "InvoiceDate");

            migrationBuilder.RenameColumn(
                name: "nmten",
                table: "VintaxInvoiceIns",
                newName: "SellerName");

            migrationBuilder.RenameColumn(
                name: "nmmst",
                table: "VintaxInvoiceIns",
                newName: "SellerAddress");

            migrationBuilder.RenameColumn(
                name: "nmdchi",
                table: "VintaxInvoiceIns",
                newName: "InvoiceNumber");

            migrationBuilder.RenameColumn(
                name: "nbten",
                table: "VintaxInvoiceIns",
                newName: "InvoiceCodeNumbber");

            migrationBuilder.RenameColumn(
                name: "nbmst",
                table: "VintaxInvoiceIns",
                newName: "InvoiceCode");

            migrationBuilder.RenameColumn(
                name: "nbdchi",
                table: "VintaxInvoiceIns",
                newName: "BuyerTax");

            migrationBuilder.RenameColumn(
                name: "khmshdon",
                table: "VintaxInvoiceIns",
                newName: "BuyerName");

            migrationBuilder.RenameColumn(
                name: "khhdon",
                table: "VintaxInvoiceIns",
                newName: "BuyerAddress");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "VintaxInvoiceIns",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "VintaxInvoiceIns",
                newName: "tgtttbso");

            migrationBuilder.RenameColumn(
                name: "SellerTax",
                table: "VintaxInvoiceIns",
                newName: "shdon");

            migrationBuilder.RenameColumn(
                name: "SellerName",
                table: "VintaxInvoiceIns",
                newName: "nmten");

            migrationBuilder.RenameColumn(
                name: "SellerAddress",
                table: "VintaxInvoiceIns",
                newName: "nmmst");

            migrationBuilder.RenameColumn(
                name: "InvoiceNumber",
                table: "VintaxInvoiceIns",
                newName: "nmdchi");

            migrationBuilder.RenameColumn(
                name: "InvoiceDate",
                table: "VintaxInvoiceIns",
                newName: "ntao");

            migrationBuilder.RenameColumn(
                name: "InvoiceCodeNumbber",
                table: "VintaxInvoiceIns",
                newName: "nbten");

            migrationBuilder.RenameColumn(
                name: "InvoiceCode",
                table: "VintaxInvoiceIns",
                newName: "nbmst");

            migrationBuilder.RenameColumn(
                name: "BuyerTax",
                table: "VintaxInvoiceIns",
                newName: "nbdchi");

            migrationBuilder.RenameColumn(
                name: "BuyerName",
                table: "VintaxInvoiceIns",
                newName: "khmshdon");

            migrationBuilder.RenameColumn(
                name: "BuyerAddress",
                table: "VintaxInvoiceIns",
                newName: "khhdon");
        }
    }
}
