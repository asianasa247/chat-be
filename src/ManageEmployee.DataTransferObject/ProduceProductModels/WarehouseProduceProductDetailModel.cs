namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class WarehouseProduceProductDetailModel
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int GoodsId { get; set; }
    public int? CarId { get; set; }
    public string? CarName { get; set; }
    public string? GoodsCode { get; set; }
    public string? GoodsName { get; set; }
    public double Quantity { get; set; }
}
