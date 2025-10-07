namespace ManageEmployee.DataTransferObject.ChartOfAccountModels;

public class ChartOfAccountModel
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public double? OpeningDebit { get; set; } = 0;
    public double? OpeningCredit { get; set; } = 0;
    public double? ArisingDebit { get; set; } = 0;
    public double? ArisingCredit { get; set; } = 0;
    public bool IsForeignCurrency { get; set; }
    public double? ArisingForeignDebit { get; set; } = 0;
    public double? ArisingForeignCredit { get; set; } = 0;
    public double? OpeningForeignDebit { get; set; } = 0;
    public double? OpeningForeignCredit { get; set; } = 0;
    public bool IsSpendAccount { get; set; }
    public int Type { get; set; }
    public int Classification { get; set; }
    public int AccGroup { get; set; }
    public string? Duration { get; set; }
    public int Protected { get; set; } = 0;
    public bool DisplayInsert { get; set; } = true;
    public bool DisplayUpdate { get; set; } = true;
    public bool DisplayDelete { get; set; }
    public long? ParentId { get; set; }
    public string? ParentRef { get; set; }
    public bool HasChild { get; set; } = false;
    public bool HasDetails { get; set; } = false;
    public double? MinimumStockQuantity { get; set; } = 0;
    public double? MaximumStockQuantity { get; set; } = 0;
    public double? StockCostPrice { get; set; } = 0;
    public double? StockSellingPrice { get; set; } = 0;
    public double? StockUnitPrice { get; set; } = 0;
    public string? StockUnit { get; set; }
    public string? WarehouseCode { get; set; }
    public string? WarehouseName { get; set; }
    public double? OpeningStockQuantity { get; set; } = 0;
    public double? ClosingForeignCredit { get; set; } = 0;
    public double? ClosingForeignDebit { get; set; } = 0;

    public double? OpeningStockQuantityNB { get; set; } = 0;
    public double? StockUnitPriceNB { get; set; } = 0;
    public double? OpeningDebitNB { get; set; } = 0;
    public double? OpeningCreditNB { get; set; } = 0;
    public double? OpeningForeignDebitNB { get; set; } = 0;
    public double? OpeningForeignCreditNB { get; set; } = 0;

    public double? ClosingDebit { get; set; } = 0;
    public double? ClosingCredit { get; set; } = 0;
    public double? ClosingStockQuantity { get; set; } = 0;
    public double? ClosingAmount { get; set; } = 0;
    public int IsInternal { get; set; } = 0;
}
