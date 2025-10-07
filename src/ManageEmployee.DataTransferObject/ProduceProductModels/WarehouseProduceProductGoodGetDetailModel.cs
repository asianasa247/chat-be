namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class WarehouseProduceProductGoodGetDetailModel
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public int GoodsId { get; set; }
    public string? GoodsCode { get; set; }
    public string? GoodsName { get; set; }
    public string? StockUnit { get; set; }
    public double QuantityRequired { get; set; }
    public double QuantityReal { get; set; }
    public double QuantityStock { get; set; }
    public string? Note { get; set; }
    public List<WarehouseProduceProductGoodDetailModel>? GoodDetails { get; set; }
}
