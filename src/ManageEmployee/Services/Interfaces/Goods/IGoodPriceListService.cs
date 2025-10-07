using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using GoodsEntity = ManageEmployee.Entities.GoodsEntities.Goods;

namespace ManageEmployee.Services.Interfaces.Goods;

public interface IGoodPriceListService
{
    Task CopyPriceList(CopyPriceListRequest request);
    Task<string> ExportGoodComparePrice(ComparePriceListRequest param);
    Task<PagingResult<GoodComparePriceResult>> GetPagingGoodComparePrice(ComparePriceListRequest request);
    Task<List<GoodsEntity>> GetPriceByPriceCode(string priceCode, List<GoodCodeModel> goodCodes);
    Task UpdatePriceList(UpdatePriceListRequest request, int year);
}
