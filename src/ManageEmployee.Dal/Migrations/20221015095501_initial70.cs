using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial70 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "FixedAsset242",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                HistoricalCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                VoucherNumber = table.Column<string>(type: "nvarchar(36)", maxLength: 36, nullable: true),
                UsedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                EndOfDepreciation = table.Column<DateTime>(type: "datetime2", nullable: true),
                LiquidationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                TotalMonth = table.Column<int>(type: "int", nullable: true),
                DepreciationOfOneDay = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                AccruedExpense = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                TotalDayDepreciationOfThisPeriod = table.Column<int>(type: "int", nullable: true),
                DepreciationOfThisPeriod = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                CarryingAmountOfLiquidationAsset = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                CarryingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                DepartmentManager = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                UserManager = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DebitCodeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                DebitDetailCodeFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                DebitDetailCodeSecondName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                CreditCodeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                CreditDetailCodeFirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                CreditDetailCodeSecondName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                DebitCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                DebitWarehouse = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                DebitDetailCodeFirst = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                DebitDetailCodeSecond = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                CreditCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                CreditWarehouse = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                CreditDetailCodeFirst = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                CreditDetailCodeSecond = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                InvoiceNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                InvoiceTaxCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                InvoiceSerial = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                InvoiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                Use = table.Column<short>(type: "smallint", nullable: false),
                DepartmentId = table.Column<int>(type: "int", nullable: true),
                UserId = table.Column<int>(type: "int", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_FixedAsset242", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "FixedAsset242");
    }
}
