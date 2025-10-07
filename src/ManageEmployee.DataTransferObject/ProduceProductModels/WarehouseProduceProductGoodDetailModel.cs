namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class WarehouseProduceProductGoodDetailModel
{
    public string? GoodsCode { get; set; }
    public string? GoodsName { get; set; }
    public string? StockUnit { get; set; }
    public double QuantityRequired { get; set; }
    public double QuantityReal { get; set; }
    public double QuantityStock { get; set; }
}