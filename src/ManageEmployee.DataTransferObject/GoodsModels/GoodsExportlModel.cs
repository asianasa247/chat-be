using ManageEmployee.Entities.GoodsEntities;

namespace ManageEmployee.DataTransferObject.GoodsModels;

public class GoodsExportlModel : Goods
{
    public double? TotalAmount { get; set; }
    public string? ListDetailName { get; set; }
    public double? Quantity { get; set; }
    public IEnumerable<string>? QrCodes { get; set; }
    public string? TaxRateName { get; set; }
    public string? GoodsQuotaName { get; set; }
    public double? TaxVAT { get; set; }
}
