using ManageEmployee.Entities.GoodsEntities;

namespace ManageEmployee.DataTransferObject.GoodsModels;

public class GoodsUpdateModel : Goods
{
    public double? OpeningDebitNB { get; set; } = 0;
    public double? StockUnitPriceNB { get; set; } = 0;
}
