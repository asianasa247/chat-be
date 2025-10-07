namespace ManageEmployee.Entities.ProduceProductEntities;

public class ManufactureOrderDetail
{
    public int Id { get; set; }
    public int ManufactureOrderId { get; set; }
    public int WarehousePlanningProduceProductDetailId { get; set; }
    public int GoodsId { get; set; }
    public double QuantityRequired { get; set; }
    public double QuantityReal { get; set; }
    public int CustomerId { get; set; }
    public int? CarId { get; set; }
    public string? CarName { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Note { get; set; }
}