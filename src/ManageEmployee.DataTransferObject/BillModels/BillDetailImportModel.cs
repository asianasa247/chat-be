namespace ManageEmployee.DataTransferObject.BillModels;

public class BillDetailImportModel
{
    public int GoodsId { get; set; }
    public string? GoodsName { get; set; }
    public string? GoodsCode { get; set; }
    public string? WareHouseName { get; set; }
    public double SalePrice { get; set; }
    public int BillQuantity { get; set; }
    public double TaxVat { get; set; }
    public double DiscountPrice { get; set; }
    public string? Image1 { get; set; }
    public string? DiscountType { get; set; }
}
