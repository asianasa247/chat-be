namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class GoodsQuotaForOrderProduceProductDetailModel
{
    public int Id { get; set; }
    public string? GoodsCode { get; set; }
    public string? GoodsName { get; set; }
    public double UnitPrice { get; set; }
    public double DiscountPrice { get; set; }
    public double TaxVAT { get; set; }
    public double QuantityQuote { get; set; }
    public double QuantityRequired { get; set; }
    public double QuantityReal { get; set; }
    public double QuantityStock { get; set; }
    public double QuantityInProgress { get; set; }
    public double QuantityDelivered { get; set; }
}
