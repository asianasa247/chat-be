namespace ManageEmployee.DataTransferObject.ChartOfAccountModels;

public class ChartAccountDropDownViewModel
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public int Type { get; set; }
    public double? ClosingCredit { get; set; }
    public double? ClosingDebit { get; set; }
    public double? ArisingDebit { get; set; }
    public double? ArisingCredit { get; set; }
    public double? OpeningCredit { get; set; }
    public double? OpeningDebit { get; set; }
    public double? ArisingForeignDebit { get; set; } = 0;
    public double? ArisingForeignCredit { get; set; } = 0;
    public double? OpeningForeignDebit { get; set; } = 0;
    public double? OpeningForeignCredit { get; set; } = 0;
    public double? ClosingForeignCredit { get; set; } = 0;
    public double? ClosingForeignDebit { get; set; } = 0;
    public string? ParentRef { get; set; }
    public string? WarehouseCode { get; set; }
    public double ClosingStockQuantity { get; set; }
    public int AccGroup { get; set; }
    public bool IsForeignCurrency { get; set; }
    public int Classification { get; set; }
    public bool DisplayInsert { get; set; }
    public string Duration { get; set; } = "";
    public int IsInternal { get; set; } = 0;
    public bool HasChild { get; set; } = false;
    public bool HasDetails { get; set; } = false;
}
