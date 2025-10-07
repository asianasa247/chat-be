namespace ManageEmployee.Entities.ProduceProductEntities;

public class OrderProduceProductDetail
{
    public int Id { get; set; }
    public int OrderProduceProductId { get; set; }
    public int ProduceProductId { get; set; }
    public int GoodsId { get; set; }
    public double QuantityRequired { get; set; }
    public double QuantityReal { get; set; }
    public double QuantityInProgress { get; set; }
    public double QuantityDelivered { get; set; }
    public bool IsProduced { get; set; }
    public bool IsDone { get; set; }
    public double UnitPrice { get; set; }
    public double DiscountPrice { get; set; }
    public double TaxVAT { get; set; }
}