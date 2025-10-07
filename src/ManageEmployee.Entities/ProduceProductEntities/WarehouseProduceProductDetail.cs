namespace ManageEmployee.Entities.ProduceProductEntities;

public class WarehouseProduceProductDetail
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int? CarId { get; set; }
    public string? CarName { get; set; }
    public int WarehouseProduceProductId { get; set; }
    public int GoodsId { get; set; }
    public bool IsProduced { get; set; }
    public double QuantityRequired { get; set; }
    public double QuantityReal { get; set; }
}