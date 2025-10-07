using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.FileModels;

namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class PlanningProduceProductGoodGetDetailModel
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public int PlanningProduceProductId { get; set; }
    public int GoodsId { get; set; }
    public string? GoodsCode { get; set; }
    public string? GoodsName { get; set; }
    public double? GoodsNec { get; set; }
    public string? StockUnit { get; set; }
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public double DiscountPrice { get; set; }
    public double TaxVAT { get; set; }
    public string? OrderProduceProductCode { get; set; }
    public double PromotionAmount { get; set; }
    public List<BillPromotionModel>? Promotions { get; set; }
    public FileDetailModel? FileDelivered { get; set; }
}