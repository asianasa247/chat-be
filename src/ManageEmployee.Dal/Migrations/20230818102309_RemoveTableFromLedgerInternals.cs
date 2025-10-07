using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class RemoveTableFromLedgerInternals : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "LedgerInternals");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "LedgerInternals",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                AmountImportWarehouse = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                AmountTransport = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                AttachVoucher = table.Column<string>(type: "nvarchar(max)", nullable: true),
                BillId = table.Column<int>(type: "int", nullable: true),
                BookDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreditCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreditCodeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreditDetailCodeFirst = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreditDetailCodeFirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreditDetailCodeSecond = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreditDetailCodeSecondName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreditWarehouse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreditWarehouseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DebitCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DebitCodeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DebitDetailCodeFirst = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DebitDetailCodeFirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DebitDetailCodeSecond = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DebitDetailCodeSecondName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DebitWarehouse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DebitWarehouseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                DepreciaDuration = table.Column<DateTime>(type: "datetime2", nullable: true),
                DepreciaMonth = table.Column<int>(type: "int", nullable: false),
                ExchangeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Group = table.Column<int>(type: "int", nullable: false),
                InvoiceAdditionalDeclarationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                InvoiceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceProductItem = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceSerial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceTaxCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IsAriseMark = table.Column<bool>(type: "bit", nullable: false),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                IsInternal = table.Column<int>(type: "int", nullable: false),
                IsVoucher = table.Column<bool>(type: "bit", nullable: false),
                LedgerId = table.Column<long>(type: "bigint", nullable: true),
                Month = table.Column<int>(type: "int", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false),
                OrginalAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalBookDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                OrginalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalCompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalCurrency = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                OrginalDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalDescriptionEN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalVoucherNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                PercentImportTax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                PercentTransport = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                ProjectCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                ReferenceAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ReferenceBookDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                ReferenceFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ReferenceVoucherNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Tab = table.Column<int>(type: "int", nullable: true),
                Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UserCreated = table.Column<long>(type: "bigint", nullable: false),
                UserDeleted = table.Column<long>(type: "bigint", nullable: false),
                UserUpdated = table.Column<long>(type: "bigint", nullable: false),
                VoucherNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Year = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LedgerInternals", x => x.Id);
            });
    }
}
