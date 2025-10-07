using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddTblLedgerProduceImports : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "LedgerProduceExportDetails",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                LedgerProduceExportId = table.Column<int>(type: "int", nullable: false),
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
                ReferenceBookDate = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false),
                IsFinished = table.Column<bool>(type: "bit", nullable: false),
                ProcedureNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                ProcedureStatusId = table.Column<int>(type: "int", nullable: true),
                ProcedureStatusName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                NoteNotAccept = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                LedgerProduceImportId = table.Column<int>(type: "int", nullable: false),
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
                ReferenceBookDate = table.Column<DateTime>(type: "datetime2", nullable: true)
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
                Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UserCreated = table.Column<int>(type: "int", nullable: false),
                UserUpdated = table.Column<int>(type: "int", nullable: false),
                IsFinished = table.Column<bool>(type: "bit", nullable: false),
                ProcedureNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                ProcedureStatusId = table.Column<int>(type: "int", nullable: true),
                ProcedureStatusName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                NoteNotAccept = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LedgerProduceImports", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "LedgerProduceExportDetails");

        migrationBuilder.DropTable(
            name: "LedgerProduceExports");

        migrationBuilder.DropTable(
            name: "LedgerProduceImportDetails");

        migrationBuilder.DropTable(
            name: "LedgerProduceImports");
    }
}
