namespace ManageEmployee.DataTransferObject.ChartOfAccountModels;

public class ChartOfAccountForCashier
{
    public long Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public int Type { get; set; } = 0;
    public string ParentRef { get; set; } = "";
    public string StockUnit { get; set; } = "";
    public double? Quantity { get; set; } = 0;
    public double? StockUnitPrice { get; set; } = 0;
    public int IsInternal { get; set; } = 0;
    public string? Image { get; set; }
    public int AccGroup { get; set; } = 0;
    public int Classification { get; set; } = 0;
    public int Protected { get; set; } = 0;
    public string Duration { get; set; } = "";
    public string? Currency { get; set; }
    public double? ExchangeRate { get; set; } = 0;
    public double? Net { get; set; } = 0;
    public string WarehouseCode { get; set; } = "";
    public string WarehouseName { get; set; } = "";
}
