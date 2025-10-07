using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial8 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "PalceResidence",
            table: "Employees",
            newName: "PlaceResidence");

        migrationBuilder.RenameColumn(
            name: "PalceOrigin",
            table: "Employees",
            newName: "PlaceOrigin");

        migrationBuilder.AlterColumn<string>(
            name: "UserUpdated",
            table: "Warehouses",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "UserCreated",
            table: "Warehouses",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "UserUpdated",
            table: "Users",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.AlterColumn<string>(
            name: "UserCreated",
            table: "Users",
            type: "nvarchar(max)",
            nullable: true,
            oldClrType: typeof(string),
            oldType: "nvarchar(max)");

        migrationBuilder.CreateTable(
            name: "Documents",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Stt = table.Column<int>(type: "int", nullable: false),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                AllowDelete = table.Column<bool>(type: "bit", nullable: false),
                Check = table.Column<bool>(type: "bit", nullable: false),
                NameDebitCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                NameCreditCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                UserCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                UserFullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UserUpdated = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Documents", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "FinalStandards",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CreditCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                PercentRatio = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                UserCreated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                UserUpdated = table.Column<string>(type: "nvarchar(max)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FinalStandards", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "LedgerInternals",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Month = table.Column<int>(type: "int", nullable: false),
                BookDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                VoucherNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsVoucher = table.Column<bool>(type: "bit", nullable: false),
                OrginalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                OrginalVoucherNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                OrginalBookDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                OrginalFullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                OrginalDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                OrginalDescriptionEN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                OrginalCompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                OrginalAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                AttachVoucher = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ReferenceVoucherNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ReferenceBookDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                ReferenceFullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ReferenceAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceAdditionalDeclarationCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceTaxCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceSerial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                InvoiceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceProductItem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitWarehouse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitDetailCodeFirst = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitDetailCodeSecond = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditWarehouse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditDetailCodeFirst = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditDetailCodeSecond = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ProjectCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DepreciaMonth = table.Column<int>(type: "int", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false),
                Group = table.Column<int>(type: "int", nullable: false),
                DepreciaDuration = table.Column<DateTime>(type: "datetime2", nullable: true),
                Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                OrginalCurrency = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                ExchangeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                IsAriseMark = table.Column<bool>(type: "bit", nullable: false),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UserCreated = table.Column<long>(type: "bigint", nullable: false),
                UserUpdated = table.Column<long>(type: "bigint", nullable: false),
                UserDeleted = table.Column<long>(type: "bigint", nullable: false),
                IsInternal = table.Column<int>(type: "int", nullable: false),
                DebitCodeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitDetailCodeFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitDetailCodeSecondName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditCodeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditDetailCodeFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditDetailCodeSecondName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitWarehouseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditWarehouseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                LedgerId = table.Column<long>(type: "bigint", nullable: true),
                BillId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_LedgerInternals", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Ledgers",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Month = table.Column<int>(type: "int", nullable: false),
                BookDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                VoucherNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsVoucher = table.Column<bool>(type: "bit", nullable: false),
                OrginalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                OrginalVoucherNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                OrginalBookDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                OrginalFullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                OrginalDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                OrginalDescriptionEN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                OrginalCompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                OrginalAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                AttachVoucher = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ReferenceVoucherNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ReferenceBookDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                ReferenceFullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ReferenceAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceAdditionalDeclarationCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceTaxCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceSerial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                InvoiceName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                InvoiceProductItem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitWarehouse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitDetailCodeFirst = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitDetailCodeSecond = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditWarehouse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditDetailCodeFirst = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditDetailCodeSecond = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ProjectCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DepreciaMonth = table.Column<int>(type: "int", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false),
                Group = table.Column<int>(type: "int", nullable: false),
                DepreciaDuration = table.Column<DateTime>(type: "datetime2", nullable: true),
                Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                OrginalCurrency = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                ExchangeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                IsAriseMark = table.Column<bool>(type: "bit", nullable: false),
                IsDelete = table.Column<bool>(type: "bit", nullable: false),
                CreateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UpdateAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                DeleteAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                UserCreated = table.Column<long>(type: "bigint", nullable: false),
                UserUpdated = table.Column<long>(type: "bigint", nullable: false),
                UserDeleted = table.Column<long>(type: "bigint", nullable: false),
                IsInternal = table.Column<int>(type: "int", nullable: false),
                DebitCodeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitDetailCodeFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitDetailCodeSecondName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditCodeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditDetailCodeFirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditDetailCodeSecondName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DebitWarehouseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditWarehouseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                BillId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Ledgers", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "TaxRates",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                DebitCodeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                CreditCodeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                Percent = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                Order = table.Column<int>(type: "int", nullable: false),
                DebitCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreditCode = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_TaxRates", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Documents");

        migrationBuilder.DropTable(
            name: "FinalStandards");

        migrationBuilder.DropTable(
            name: "LedgerInternals");

        migrationBuilder.DropTable(
            name: "Ledgers");

        migrationBuilder.DropTable(
            name: "TaxRates");

        migrationBuilder.RenameColumn(
            name: "PlaceResidence",
            table: "Employees",
            newName: "PalceResidence");

        migrationBuilder.RenameColumn(
            name: "PlaceOrigin",
            table: "Employees",
            newName: "PalceOrigin");

        migrationBuilder.AlterColumn<string>(
            name: "UserUpdated",
            table: "Warehouses",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "UserCreated",
            table: "Warehouses",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "UserUpdated",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);

        migrationBuilder.AlterColumn<string>(
            name: "UserCreated",
            table: "Users",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "nvarchar(max)",
            oldNullable: true);
    }
}
