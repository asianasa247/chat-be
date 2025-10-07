using ManageEmployee.DataTransferObject.GoodsModels.GoodsQuotaModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.GoodsEntities;

namespace ManageEmployee.Services.Interfaces.Goods;

public interface IGoodsQuotaStepService
{
    Task<GoodsQuotaStep> Create(GoodsQuotaStepModel request);
    Task Delete(int id);
    Task<IEnumerable<GoodsQuotaStepListModel>> GetAll();
    Task<GoodsQuotaStepModel> GetDetail(int id);
    Task<PagingResult<GoodsQuotaStep>> GetPaging(PagingRequestModel searchRequest);
    Task Update(GoodsQuotaStepModel request);
}
