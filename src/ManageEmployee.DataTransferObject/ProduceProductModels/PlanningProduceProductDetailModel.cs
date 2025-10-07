namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class PlanningProduceProductDetailModel
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int PlanningProduceProductId { get; set; }
    public int GoodsId { get; set; }
    public int? CarId { get; set; }
    public string? CarName { get; set; }
    public string? GoodsCode { get; set; }
    public string? GoodsName { get; set; }
    public double? GoodsNet { get; set; }
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double DiscountPrice { get; set; }
    public double TaxVAT { get; set; }
}
