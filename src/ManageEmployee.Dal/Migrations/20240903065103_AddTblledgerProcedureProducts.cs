using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblledgerProcedureProducts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "LedgerProduceExportDetails");

        migrationBuilder.DropTable(
            name: "LedgerProduceExports");

        migrationBuilder.DropTable(
            name: "LedgerProduceImportDetails");

        migrationBuilder.DropTable(
            name: "LedgerProduceImports");

        migrationBuilder.CreateTable(
            name: "LedgerProcedureProducts",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                FileStr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false),
                IsFinished = table.Column<bool>(type: "bit", nullable: false),
                ProcedureNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                ProcedureStatusId = table.Column<int>(type: "int", nullable: true),
                ProcedureStatusName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                NoteNotAccept = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LedgerProcedureProducts", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "LedgerProduceProductDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                LedgerProduceProductId = table.Column<int>(type: "int", nullable: false),
                LedgerSourceId = table.Column<int>(type: "int", nullable: false),
                LedgerDestinationId = table.Column<int>(type: "int", nullable: false),
                Amount = table.Column<double>(type: "float", nullable: false),
                DebitCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DebitWarehouse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DebitDetailCodeFirst = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DebitDetailCodeSecond = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreditCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreditWarehouse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreditDetailCodeFirst = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreditDetailCodeSecond = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Quantity = table.Column<double>(type: "float", nullable: false),
                UnitPrice = table.Column<double>(type: "float", nullable: false),
                OrginalCurrency = table.Column<double>(type: "float", nullable: false),
                DebitCodeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DebitDetailCodeFirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DebitDetailCodeSecondName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreditCodeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreditDetailCodeFirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreditDetailCodeSecondName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DebitWarehouseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreditWarehouseName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Month = table.Column<int>(type: "int", nullable: false),
                BookDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                VoucherNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IsVoucher = table.Column<bool>(type: "bit", nullable: false),
                OrginalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalVoucherNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalBookDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                OrginalFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalDescriptionEN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalCompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                AttachVoucher = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ReferenceVoucherNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ReferenceBookDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                InvoiceCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceAdditionalDeclarationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceTaxCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceSerial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                InvoiceName = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LedgerProduceProductDetails", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "LedgerProcedureProducts");

        migrationBuilder.DropTable(
            name: "LedgerProduceProductDetails");

        migrationBuilder.CreateTable(
            name: "LedgerProduceExportDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Amount = table.Column<double>(type: "float", nullable: false),
                AttachVoucher = table.Column<string>(type: "nvarchar(max)", nullable: true),
                BookDate = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                InvoiceAdditionalDeclarationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                InvoiceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceSerial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceTaxCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IsVoucher = table.Column<bool>(type: "bit", nullable: false),
                LedgerDestinationId = table.Column<int>(type: "int", nullable: false),
                LedgerProduceExportId = table.Column<int>(type: "int", nullable: false),
                LedgerSourceId = table.Column<int>(type: "int", nullable: false),
                Month = table.Column<int>(type: "int", nullable: false),
                OrginalAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalBookDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                OrginalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalCompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalCurrency = table.Column<double>(type: "float", nullable: false),
                OrginalDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalDescriptionEN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalVoucherNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Quantity = table.Column<double>(type: "float", nullable: false),
                ReferenceBookDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                ReferenceVoucherNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UnitPrice = table.Column<double>(type: "float", nullable: false),
                VoucherNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LedgerProduceExportDetails", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "LedgerProduceExports",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                FileStr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IsFinished = table.Column<bool>(type: "bit", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                NoteNotAccept = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ProcedureNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                ProcedureStatusId = table.Column<int>(type: "int", nullable: true),
                ProcedureStatusName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LedgerProduceExports", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "LedgerProduceImportDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Amount = table.Column<double>(type: "float", nullable: false),
                AttachVoucher = table.Column<string>(type: "nvarchar(max)", nullable: true),
                BookDate = table.Column<DateTime>(type: "datetime2", nullable: true),
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
                InvoiceAdditionalDeclarationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                InvoiceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceSerial = table.Column<string>(type: "nvarchar(max)", nullable: true),
                InvoiceTaxCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IsVoucher = table.Column<bool>(type: "bit", nullable: false),
                LedgerDestinationId = table.Column<int>(type: "int", nullable: false),
                LedgerProduceImportId = table.Column<int>(type: "int", nullable: false),
                LedgerSourceId = table.Column<int>(type: "int", nullable: false),
                Month = table.Column<int>(type: "int", nullable: false),
                OrginalAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalBookDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                OrginalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalCompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalCurrency = table.Column<double>(type: "float", nullable: false),
                OrginalDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalDescriptionEN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalFullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                OrginalVoucherNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Quantity = table.Column<double>(type: "float", nullable: false),
                ReferenceBookDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                ReferenceVoucherNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UnitPrice = table.Column<double>(type: "float", nullable: false),
                VoucherNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LedgerProduceImportDetails", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "LedgerProduceImports",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                FileStr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IsFinished = table.Column<bool>(type: "bit", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                NoteNotAccept = table.Column<string>(type: "nvarchar(max)", nullable: true),
                ProcedureNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                ProcedureStatusId = table.Column<int>(type: "int", nullable: true),
                ProcedureStatusName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LedgerProduceImports", x => x.Id);
            });
    }
}
