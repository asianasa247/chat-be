namespace ManageEmployee.Entities.ProduceProductEntities;

public class ProduceProductDetail
{
    public int Id { get; set; }
    public int ProduceProductId { get; set; }
    public int GoodsId { get; set; }
    public double QuantityRequired { get; set; }
    public double QuantityReal { get; set; }
    public int CustomerId { get; set; }
    public int? CarId { get; set; }
    public string? CarName { get; set; }
}