using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class initial2 : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "Order",
            table: "Targets",
            type: "int",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<bool>(
            name: "IsDelete",
            table: "Branchs",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.CreateTable(
            name: "ChartOfAccountGroupLinks",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                CodeChartOfAccountGroup = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CodeChartOfAccount = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ChartOfAccountGroupLinks", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ChartOfAccountGroups",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ChartOfAccountGroups", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ChartOfAccounts",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                OpeningDebit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                OpeningCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                ArisingDebit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                ArisingCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                IsForeignCurrency = table.Column<bool>(type: "bit", nullable: false),
                OpeningForeignDebit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                OpeningForeignCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                ArisingForeignDebit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                ArisingForeignCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                Duration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ClosingDebit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                ClosingCredit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                Currency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ExchangeRate = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                AccGroup = table.Column<int>(type: "int", nullable: false),
                Classification = table.Column<int>(type: "int", nullable: false),
                IsProtected = table.Column<bool>(type: "bit", nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                HasChild = table.Column<bool>(type: "bit", nullable: false),
                HasDetails = table.Column<bool>(type: "bit", nullable: false),
                ParentRef = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DisplayInsert = table.Column<bool>(type: "bit", nullable: false),
                DisplayDelete = table.Column<bool>(type: "bit", nullable: false),
                StockUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                OpeningStockQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                ArisingStockQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                StockUnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                WarehouseCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                OpeningDebitNB = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                OpeningCreditNB = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                ArisingDebitNB = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                ArisingCreditNB = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                OpeningForeignDebitNB = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                OpeningForeignCreditNB = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                ArisingForeignDebitNB = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                ArisingForeignCreditNB = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                OpeningStockQuantityNB = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                ArisingStockQuantityNB = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                StockUnitPriceNB = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ChartOfAccounts", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ChartOfAcountTests",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ChartOfAcountTests", x => x.Id);
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ChartOfAccountGroupLinks");

        migrationBuilder.DropTable(
            name: "ChartOfAccountGroups");

        migrationBuilder.DropTable(
            name: "ChartOfAccounts");

        migrationBuilder.DropTable(
            name: "ChartOfAcountTests");

        migrationBuilder.DropColumn(
            name: "Order",
            table: "Targets");

        migrationBuilder.DropColumn(
            name: "IsDelete",
            table: "Branchs");
    }
}
