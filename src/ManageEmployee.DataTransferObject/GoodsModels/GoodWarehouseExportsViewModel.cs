namespace ManageEmployee.DataTransferObject.GoodsModels;

public class GoodWarehouseExportsViewModel
{
    public int Id { get; set; }
    public double Quantity { get; set; }
    public int BillId { get; set; }
    public string? GoodsCode { get; set; }
    public string? GoodsName { get; set; }
    public string? Warehouse { get; set; }
    public string? WarehouseName { get; set; }
    public DateTime? CreateAt { get; set; }
    public DateTime? DateExpiration { get; set; }
    public string? QrCode { get; set; }
    public string? GoodName { get; set; }
    public string? GoodCode { get; set; }
    public string? OrginalVoucherNumber { get; set; }// ma chung tu
    public int? Order { get; set; }
}
