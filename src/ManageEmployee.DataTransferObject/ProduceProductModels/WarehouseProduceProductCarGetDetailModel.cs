namespace ManageEmployee.DataTransferObject.ProduceProductModels;

public class WarehouseProduceProductCarGetDetailModel
{
    public int? CarId { get; set; }
    public string? CarName { get; set; }
    public List<WarehouseProduceProductGoodGetDetailModel>? Goods { get; set; }
}
