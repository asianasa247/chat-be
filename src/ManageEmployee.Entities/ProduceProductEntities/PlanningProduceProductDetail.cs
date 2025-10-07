namespace ManageEmployee.Entities.ProduceProductEntities;

public class PlanningProduceProductDetail
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int? CarId { get; set; }
    public string? CarName { get; set; }
    public int PlanningProduceProductId { get; set; }
    public int GoodsId { get; set; }
    public double Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public int OrderProduceProductDetailId { get; set; }
    public double UnitPrice { get; set; }
    public double DiscountPrice { get; set; }
    public double TaxVAT { get; set; }
    public int? OrderProduceProductId { get; set; }
    public string? OrderProduceProductCode { get; set; }
    public string? FileDeliveredStr { get; set; }
    public bool IsCanceled { get; set; }
    public string? DeliveryCode { get; set; }
    public DateTime Date { get; set; }
    public bool ShouldExport { get; set; }
    public string? CreateFromTable { get; set; }

}