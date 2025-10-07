namespace ManageEmployee.DataTransferObject.P_ProcedureView;

public class P_Inventory_Item_ViewModel
{
    public int Id { get; set; }
    public int P_InventoryId { get; set; }
    public string? GoodsCode { get; set; }
    public string? GoodsName { get; set; }
    public double InputQuantity { get; set; } = 0;
    public double OutputQuantity { get; set; } = 0;
    public double Quantity { get; set; } = 0;
    public double QuantityReal { get; set; } = 0;
    public string? Note { get; set; }
    public DateTime? DateExpiration { get; set; }
    public string? QrCode { get; set; }

}
