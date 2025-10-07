namespace ManageEmployee.DataTransferObject.GoodsModels;

public class GoodComparePriceItem
{
    public string? Code { get; set; }
    public double SalePrice { get; set; }
    public double? TaxVAT { get; set; }
    public double? Amount { get; set; }
    public double DifferentSalePrice { get; set; }
}
