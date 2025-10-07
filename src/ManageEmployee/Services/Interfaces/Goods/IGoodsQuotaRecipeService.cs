using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.GoodsEntities;

namespace ManageEmployee.Services.Interfaces.Goods;

public interface IGoodsQuotaRecipeService
{
    Task Create(GoodsQuotaRecipe request);
    Task Delete(int id);
    Task<IEnumerable<GoodsQuotaRecipe>> GetAll();
    Task<PagingResult<GoodsQuotaRecipe>> GetPaging(PagingRequestModel searchRequest);
    Task Update(GoodsQuotaRecipe request);
}
