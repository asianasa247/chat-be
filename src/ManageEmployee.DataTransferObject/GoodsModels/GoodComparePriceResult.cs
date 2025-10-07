namespace ManageEmployee.DataTransferObject.GoodsModels;

public class GoodComparePriceResult
{
    public string? Code { get; set; }
    public string? Name { get; set; }
    public string? Image1 { get; set; }
    public string? WarehouseName { get; set; }
    public List<GoodComparePriceItem>? listItem { get; set; }
}
