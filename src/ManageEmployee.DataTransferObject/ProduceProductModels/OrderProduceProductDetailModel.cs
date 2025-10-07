namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class OrderProduceProductDetailModel
{
    public int Id { get; set; }
    public int GoodsId { get; set; }
    public string? GoodsCode { get; set; }
    public string? GoodsName { get; set; }
    public double QuantityRequired { get; set; }
    public double QuantityReal { get; set; }
    public double QuantityStock { get; set; }
    public double PlanningQuantity { get; set; }
    public double QuantityDelivered { get; set; }
    public double QuantityInProgress { get; set; }
    public bool IsProduced { get; set; }
    public bool IsDeleted { get; set; }

    public double UnitPrice { get; set; }
    public double DiscountPrice { get; set; }
    public double TaxVAT { get; set; }
    public string? ProcedureNumber { get; set; }
    public string? ProcedureStatusName { get; set; }
    public List<GoodsQuotaForOrderProduceProductDetailModel>? GoodsQuotes { get; set; }
}
