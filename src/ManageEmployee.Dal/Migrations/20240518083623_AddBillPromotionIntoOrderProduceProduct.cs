using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManageEmployee.Dal.Migrations;

public partial class AddBillPromotionIntoOrderProduceProduct : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "BillId",
            table: "BillPromotions",
            newName: "TableId");

        migrationBuilder.AlterColumn<double>(
            name: "ToAmountAuto",
            table: "TillManagers",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "FromAmount",
            table: "TillManagers",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "AmountDifferent",
            table: "TillManagers",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "Percent",
            table: "TaxRates",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "Value",
            table: "P_SalaryAdvance_Item",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "QuantityReal",
            table: "P_Inventory_Items",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "Quantity",
            table: "P_Inventory_Items",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "OutputQuantity",
            table: "P_Inventory_Items",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "InputQuantity",
            table: "P_Inventory_Items",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AddColumn<double>(
            name: "TotalAmount",
            table: "OrderProduceProducts",
            type: "float",
            nullable: false,
            defaultValue: 0.0);

        migrationBuilder.AlterColumn<double>(
            name: "UnitPrice",
            table: "OrderProduceProductDetails",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "TaxVAT",
            table: "OrderProduceProductDetails",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "DiscountPrice",
            table: "OrderProduceProductDetails",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "TaxVAT",
            table: "OrderDetail",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "Price",
            table: "OrderDetail",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "TotalPricePaid",
            table: "Order",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "TotalPriceDiscount",
            table: "Order",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "TotalPrice",
            table: "Order",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "ToValue",
            table: "MenuKpis",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "FromValue",
            table: "MenuKpis",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "UnitPrice",
            table: "Ledgers",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "Quantity",
            table: "Ledgers",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "PercentTransport",
            table: "Ledgers",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "PercentImportTax",
            table: "Ledgers",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "OrginalCurrency",
            table: "Ledgers",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "ExchangeRate",
            table: "Ledgers",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,4)");

        migrationBuilder.AlterColumn<double>(
            name: "AmountTransport",
            table: "Ledgers",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "AmountImportWarehouse",
            table: "Ledgers",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "Amount",
            table: "Ledgers",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "OutputQuantity",
            table: "Inventory",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "InputQuantity",
            table: "Inventory",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "CloseQuantityReal",
            table: "Inventory",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "CloseQuantity",
            table: "Inventory",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "Quantity",
            table: "GoodWarehousesPositions",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "QuantityInput",
            table: "GoodWarehouses",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "Quantity",
            table: "GoodWarehouses",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "Quantity",
            table: "GoodWarehouseExport",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "Quantity",
            table: "GoodsQuotaDetails",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "SalePrice",
            table: "GoodsPriceList",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "Price",
            table: "GoodsPriceList",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "DiscountPrice",
            table: "GoodsPriceList",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "WebPriceVietNam",
            table: "Goods",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "WebPriceKorea",
            table: "Goods",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "WebPriceEnglish",
            table: "Goods",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "WebDiscountVietNam",
            table: "Goods",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "WebDiscountKorea",
            table: "Goods",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "WebDiscountEnglish",
            table: "Goods",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "SalePrice",
            table: "Goods",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "Price",
            table: "Goods",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "OpeningStockQuantityNB",
            table: "Goods",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "DiscountPrice",
            table: "Goods",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "UnitPrice",
            table: "GoodDetails",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "Quantity",
            table: "GoodDetails",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "Amount",
            table: "GoodDetails",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "UnitPrice",
            table: "FixedAssetUser",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "Quantity",
            table: "FixedAssetUser",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "HistoricalCost",
            table: "FixedAssetUser",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "DepreciationOfThisPeriod",
            table: "FixedAssetUser",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "DepreciationOfOneDay",
            table: "FixedAssetUser",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "CarryingAmountOfLiquidationAsset",
            table: "FixedAssetUser",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "CarryingAmount",
            table: "FixedAssetUser",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "AccruedExpense",
            table: "FixedAssetUser",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "UnitPrice",
            table: "FixedAssets",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "Quantity",
            table: "FixedAssets",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "HistoricalCost",
            table: "FixedAssets",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "DepreciationOfThisPeriod",
            table: "FixedAssets",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "DepreciationOfOneDay",
            table: "FixedAssets",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "CarryingAmountOfLiquidationAsset",
            table: "FixedAssets",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "CarryingAmount",
            table: "FixedAssets",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "AccruedExpense",
            table: "FixedAssets",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "UnitPrice",
            table: "FixedAsset242",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "Quantity",
            table: "FixedAsset242",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "HistoricalCost",
            table: "FixedAsset242",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "DepreciationOfThisPeriod",
            table: "FixedAsset242",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "DepreciationOfOneDay",
            table: "FixedAsset242",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "CarryingAmountOfLiquidationAsset",
            table: "FixedAsset242",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "CarryingAmount",
            table: "FixedAsset242",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "AccruedExpense",
            table: "FixedAsset242",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "PercentRatio",
            table: "FinalStandards",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "UnitPrice",
            table: "CustomerQuote_Detail",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "TaxVAT",
            table: "CustomerQuote_Detail",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "Quantity",
            table: "CustomerQuote_Detail",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "DiscountPrice",
            table: "CustomerQuote_Detail",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "StockUnitPriceNB",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "StockUnitPrice",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "OpeningStockQuantityNB",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "OpeningStockQuantity",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "OpeningForeignDebitNB",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "OpeningForeignDebit",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "OpeningForeignCreditNB",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "OpeningForeignCredit",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "OpeningDebitNB",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "OpeningDebit",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "OpeningCreditNB",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "OpeningCredit",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "ExchangeRate",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "ArisingStockQuantityNB",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "ArisingStockQuantity",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "ArisingForeignDebitNB",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "ArisingForeignDebit",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "ArisingForeignCreditNB",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "ArisingForeignCredit",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "ArisingDebitNB",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "ArisingDebit",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "ArisingCreditNB",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "ArisingCredit",
            table: "ChartOfAccounts",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "VatRate",
            table: "Bills",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "Vat",
            table: "Bills",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "TotalAmount",
            table: "Bills",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "Surcharge",
            table: "Bills",
            type: "float",
            nullable: true,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)",
            oldNullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "DiscountPrice",
            table: "Bills",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "AmountSendToCus",
            table: "Bills",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "AmountRefund",
            table: "Bills",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "AmountReceivedByCus",
            table: "Bills",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AddColumn<string>(
            name: "TableName",
            table: "BillPromotions",
            type: "nvarchar(max)",
            nullable: true);

        migrationBuilder.AlterColumn<double>(
            name: "UnitPrice",
            table: "BillDetails",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "TaxVAT",
            table: "BillDetails",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "Price",
            table: "BillDetails",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");

        migrationBuilder.AlterColumn<double>(
            name: "DiscountPrice",
            table: "BillDetails",
            type: "float",
            nullable: false,
            oldClrType: typeof(decimal),
            oldType: "decimal(18,2)");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "TotalAmount",
            table: "OrderProduceProducts");

        migrationBuilder.DropColumn(
            name: "TableName",
            table: "BillPromotions");

        migrationBuilder.RenameColumn(
            name: "TableId",
            table: "BillPromotions",
            newName: "BillId");

        migrationBuilder.AlterColumn<decimal>(
            name: "ToAmountAuto",
            table: "TillManagers",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "FromAmount",
            table: "TillManagers",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "AmountDifferent",
            table: "TillManagers",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "Percent",
            table: "TaxRates",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "Value",
            table: "P_SalaryAdvance_Item",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "QuantityReal",
            table: "P_Inventory_Items",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "Quantity",
            table: "P_Inventory_Items",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "OutputQuantity",
            table: "P_Inventory_Items",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "InputQuantity",
            table: "P_Inventory_Items",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "UnitPrice",
            table: "OrderProduceProductDetails",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "TaxVAT",
            table: "OrderProduceProductDetails",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "DiscountPrice",
            table: "OrderProduceProductDetails",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "TaxVAT",
            table: "OrderDetail",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "Price",
            table: "OrderDetail",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "TotalPricePaid",
            table: "Order",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "TotalPriceDiscount",
            table: "Order",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "TotalPrice",
            table: "Order",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "ToValue",
            table: "MenuKpis",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "FromValue",
            table: "MenuKpis",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "UnitPrice",
            table: "Ledgers",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "Quantity",
            table: "Ledgers",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "PercentTransport",
            table: "Ledgers",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "PercentImportTax",
            table: "Ledgers",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "OrginalCurrency",
            table: "Ledgers",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "ExchangeRate",
            table: "Ledgers",
            type: "decimal(18,4)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "AmountTransport",
            table: "Ledgers",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "AmountImportWarehouse",
            table: "Ledgers",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "Amount",
            table: "Ledgers",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "OutputQuantity",
            table: "Inventory",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "InputQuantity",
            table: "Inventory",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "CloseQuantityReal",
            table: "Inventory",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "CloseQuantity",
            table: "Inventory",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "Quantity",
            table: "GoodWarehousesPositions",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "QuantityInput",
            table: "GoodWarehouses",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "Quantity",
            table: "GoodWarehouses",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "Quantity",
            table: "GoodWarehouseExport",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "Quantity",
            table: "GoodsQuotaDetails",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "SalePrice",
            table: "GoodsPriceList",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "Price",
            table: "GoodsPriceList",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "DiscountPrice",
            table: "GoodsPriceList",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "WebPriceVietNam",
            table: "Goods",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "WebPriceKorea",
            table: "Goods",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "WebPriceEnglish",
            table: "Goods",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "WebDiscountVietNam",
            table: "Goods",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "WebDiscountKorea",
            table: "Goods",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "WebDiscountEnglish",
            table: "Goods",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "SalePrice",
            table: "Goods",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "Price",
            table: "Goods",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "OpeningStockQuantityNB",
            table: "Goods",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "DiscountPrice",
            table: "Goods",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "UnitPrice",
            table: "GoodDetails",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "Quantity",
            table: "GoodDetails",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "Amount",
            table: "GoodDetails",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "UnitPrice",
            table: "FixedAssetUser",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "Quantity",
            table: "FixedAssetUser",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "HistoricalCost",
            table: "FixedAssetUser",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "DepreciationOfThisPeriod",
            table: "FixedAssetUser",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "DepreciationOfOneDay",
            table: "FixedAssetUser",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "CarryingAmountOfLiquidationAsset",
            table: "FixedAssetUser",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "CarryingAmount",
            table: "FixedAssetUser",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "AccruedExpense",
            table: "FixedAssetUser",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "UnitPrice",
            table: "FixedAssets",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "Quantity",
            table: "FixedAssets",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "HistoricalCost",
            table: "FixedAssets",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "DepreciationOfThisPeriod",
            table: "FixedAssets",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "DepreciationOfOneDay",
            table: "FixedAssets",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "CarryingAmountOfLiquidationAsset",
            table: "FixedAssets",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "CarryingAmount",
            table: "FixedAssets",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "AccruedExpense",
            table: "FixedAssets",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "UnitPrice",
            table: "FixedAsset242",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "Quantity",
            table: "FixedAsset242",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "HistoricalCost",
            table: "FixedAsset242",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "DepreciationOfThisPeriod",
            table: "FixedAsset242",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "DepreciationOfOneDay",
            table: "FixedAsset242",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "CarryingAmountOfLiquidationAsset",
            table: "FixedAsset242",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "CarryingAmount",
            table: "FixedAsset242",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "AccruedExpense",
            table: "FixedAsset242",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "PercentRatio",
            table: "FinalStandards",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "UnitPrice",
            table: "CustomerQuote_Detail",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "TaxVAT",
            table: "CustomerQuote_Detail",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "Quantity",
            table: "CustomerQuote_Detail",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "DiscountPrice",
            table: "CustomerQuote_Detail",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "StockUnitPriceNB",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "StockUnitPrice",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "OpeningStockQuantityNB",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "OpeningStockQuantity",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "OpeningForeignDebitNB",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "OpeningForeignDebit",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "OpeningForeignCreditNB",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "OpeningForeignCredit",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "OpeningDebitNB",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "OpeningDebit",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "OpeningCreditNB",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "OpeningCredit",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "ExchangeRate",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "ArisingStockQuantityNB",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "ArisingStockQuantity",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "ArisingForeignDebitNB",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "ArisingForeignDebit",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "ArisingForeignCreditNB",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "ArisingForeignCredit",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "ArisingDebitNB",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "ArisingDebit",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "ArisingCreditNB",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "ArisingCredit",
            table: "ChartOfAccounts",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "VatRate",
            table: "Bills",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "Vat",
            table: "Bills",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "TotalAmount",
            table: "Bills",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "Surcharge",
            table: "Bills",
            type: "decimal(18,2)",
            nullable: true,
            oldClrType: typeof(double),
            oldType: "float",
            oldNullable: true);

        migrationBuilder.AlterColumn<decimal>(
            name: "DiscountPrice",
            table: "Bills",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "AmountSendToCus",
            table: "Bills",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "AmountRefund",
            table: "Bills",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "AmountReceivedByCus",
            table: "Bills",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "UnitPrice",
            table: "BillDetails",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "TaxVAT",
            table: "BillDetails",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "Price",
            table: "BillDetails",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");

        migrationBuilder.AlterColumn<decimal>(
            name: "DiscountPrice",
            table: "BillDetails",
            type: "decimal(18,2)",
            nullable: false,
            oldClrType: typeof(double),
            oldType: "float");
    }
}
