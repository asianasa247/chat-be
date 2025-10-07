using ManageEmployee.DataTransferObject.GoodsModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.Goods;

public interface IGoodsPromotionService
{
    Task Create(GoodsPromotionSetterModel param);
    Task Delete(int id);
    Task<GoodsPromotionGetDetailModel> GetById(int id, int year);
    Task<IEnumerable<GoodsPromotionGetListModel>> GetList();
    Task<IEnumerable<GoodsPromotionDetailForSaleModel>> GetListForSale();
    Task<PagingResult<GoodsPromotionModel>> GetPaging(PagingRequestModel param);
    Task Update(GoodsPromotionSetterModel param);
}
