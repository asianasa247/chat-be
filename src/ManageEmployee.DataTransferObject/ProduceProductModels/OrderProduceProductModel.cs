using ManageEmployee.DataTransferObject.BillModels;

namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class OrderProduceProductModel
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public bool IsFinished { get; set; }
    public bool IsCanceled { get; set; }
    public bool IsSpecialOrder { get; set; }
    public List<OrderProduceProductDetailModel>? Items { get; set; }
    public List<BillPromotionModel>? BillPromotions { get; set; }
}
