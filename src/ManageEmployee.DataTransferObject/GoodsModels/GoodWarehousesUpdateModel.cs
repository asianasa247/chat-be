namespace ManageEmployee.DataTransferObject.GoodsModels;

public class GoodWarehousesUpdateModel
{
    public int Id { get; set; }
    public string? MenuType { get; set; }
    public string? GoodsType { get; set; }
    public double Quantity { get; set; }
    public string? Note { get; set; }
    public int Status { get; set; }
    public string? Account { get; set; }
    public string? AccountName { get; set; }
    public string? Warehouse { get; set; }
    public string? WarehouseName { get; set; }
    public string? Detail1 { get; set; }
    public string? DetailName1 { get; set; }
    public string? Detail2 { get; set; }
    public string? DetailName2 { get; set; }
    public string? Image1 { get; set; }
    public DateTime? DateManufacture { get; set; }
    public DateTime? DateExpiration { get; set; }
    public string? Type { get; set; }
    public string? OrginalVoucherNumber { get; set; }// ma chung tu
    public int? Order { get; set; }
    public List<GoodWarehousesPositionUpdateModel>? Positions { get; set; }
    public int? LedgerId { get; set; }
}
