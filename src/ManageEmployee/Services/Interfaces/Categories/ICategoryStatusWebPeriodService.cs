using ManageEmployee.DataTransferObject.CategoryModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.CategoryEntities;

namespace ManageEmployee.Services.Interfaces.Categories;

public interface ICategoryStatusWebPeriodService
{
    Task<PagingResult<CategoryStatusWebPeriodModel>> GetAll(int pageIndex, int pageSize);

    Task<IEnumerable<CategoryStatusWebPeriod>> GetAll();

    Task<IEnumerable<Category>> GetListCategoryStatusWeb();

    Task<CategoryStatusWebPeriodModel> GetById(int id);

    Task Create(CategoryStatusWebPeriodModel form);

    Task Update(CategoryStatusWebPeriodModel form);

    Task Delete(int id);

    Task<IEnumerable<CategoryStatusWebPeriodGoodShowWebModel>> GetGoodShowWeb(string code);

    Task<CategoryStatusWebPeriodModel> GetDealsOfDay();
}
