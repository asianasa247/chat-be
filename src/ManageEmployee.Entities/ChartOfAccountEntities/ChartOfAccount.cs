using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.ChartOfAccountEntities;

public class ChartOfAccount
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public double? OpeningDebit { get; set; } = 0;
    public double? OpeningCredit { get; set; } = 0;
    public double? ArisingDebit { get; set; } = 0;
    public double? ArisingCredit { get; set; } = 0;
    public bool IsForeignCurrency { get; set; } = false;
    public double? OpeningForeignDebit { get; set; } = 0;
    public double? OpeningForeignCredit { get; set; } = 0;
    public double? ArisingForeignDebit { get; set; } = 0;
    public double? ArisingForeignCredit { get; set; } = 0;
    public string Duration { get; set; } = "";
    public string? Currency { get; set; }
    public double? ExchangeRate { get; set; } = 0;
    public int AccGroup { get; set; } = 0;
    public int Classification { get; set; } = 0;
    public int Protected { get; set; } = 0;
    public int Type { get; set; } = 0;
    public bool HasChild { get; set; } = false;
    public bool HasDetails { get; set; } = false;
    public string ParentRef { get; set; } = "";
    public bool DisplayInsert { get; set; } = true;
    public bool DisplayDelete { get; set; } = true;
    public string StockUnit { get; set; } = "";
    public double? OpeningStockQuantity { get; set; } = 0;
    public double? ArisingStockQuantity { get; set; } = 0;
    public double? StockUnitPrice { get; set; } = 0;
    public string WarehouseCode { get; set; } = "";
    [StringLength(255)]
    public string WarehouseName { get; set; } = "";
    public double? OpeningDebitNB { get; set; } = 0;
    public double? OpeningCreditNB { get; set; } = 0;
    public double? ArisingDebitNB { get; set; } = 0;
    public double? ArisingCreditNB { get; set; } = 0;
    public double? OpeningForeignDebitNB { get; set; } = 0;
    public double? OpeningForeignCreditNB { get; set; } = 0;
    public double? ArisingForeignDebitNB { get; set; } = 0;
    public double? ArisingForeignCreditNB { get; set; } = 0;
    public double? OpeningStockQuantityNB { get; set; } = 0;
    public double? ArisingStockQuantityNB { get; set; } = 0;
    public double? StockUnitPriceNB { get; set; } = 0;
    public int Year { get; set; } = 0;
    public int IsInternal { get; set; } = 0;
}
