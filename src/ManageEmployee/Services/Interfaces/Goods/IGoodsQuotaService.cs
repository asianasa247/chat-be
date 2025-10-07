using ManageEmployee.DataTransferObject.GoodsModels.GoodsQuotaModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.Goods;

public interface IGoodsQuotaService
{
    Task Create(GoodsQuotaModel param, int userId);
    Task<GoodsQuotaModel> GetDetail(int id);
    Task<PagingResult<GoodsQuotaPagingGetterModel>> GetPaging(GoodsQuotasRequestModel searchRequest, int userId);
    Task GoodsQuotaForGoodsDetail(List<int> goodIds, int goodsQuotaId);
    Task Update(GoodsQuotaModel param, int userId);
    Task<IEnumerable<GoodsQuotaGetAllModel>> GetAll();
    Task<string> GetProcedureNumber();
    Task NotAccept(int id, int userId);
    Task Accept(int id, int userId);
}
