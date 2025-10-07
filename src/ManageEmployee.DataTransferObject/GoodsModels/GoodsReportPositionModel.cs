namespace ManageEmployee.DataTransferObject.GoodsModels;

public class GoodsReportPositionModel
{
    public string? Account { get; set; }
    public string? AccountName { get; set; }
    public string? Warehouse { get; set; }
    public string? WarehouseName { get; set; }
    public string? Detail1 { get; set; }
    public string? DetailName1 { get; set; }
    public string? Detail2 { get; set; }
    public string? DetailName2 { get; set; }
    public string? StockUnit { get; set; }
    public double? Quantity { get; set; }
    public List<string>? Positions { get; set; }
}
