namespace ManageEmployee.DataTransferObject.SupplyModels;

public class RequestEquipmentOrderDetailModel
{
    public int Id { get; set; }
    public string? GoodName { get; set; }
    public string? GoodCategory { get; set; }
    public string? GoodProducer { get; set; }
    public string? GoodCatalog { get; set; }
    public string? GoodUnit { get; set; }
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public string? GoodType { get; set; }
    public string? Note { get; set; }
    public string? FileStr { get; set; }
}