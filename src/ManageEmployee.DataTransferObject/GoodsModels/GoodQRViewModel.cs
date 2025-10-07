namespace ManageEmployee.DataTransferObject.GoodsModels;

public class GoodQRViewModel
{
    public string? Account { get; set; }
    public string? Detail1 { get; set; }
    public string? Detail2 { get; set; }
    public string? WareHouseCode { get; set; }
    public DateTime? DateExpiration { get; set; }
    public int GoodWarehouseId { get; set; }
    public int Quantity { get; set; }
}
