using ManageEmployee.DataTransferObject.BillModels;

namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class OrderProduceProductCreateModel
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public List<OrderProduceProductDetailSetterModel>? Items { get; set; }
    public List<BillPromotionModel>? BillPromotions { get; set; }
}
