using ManageEmployee.DataTransferObject.CategoryModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Entities.CategoryEntities;

namespace ManageEmployee.Services.Interfaces.Categories;

public interface ICategoryService
{
    Task<IEnumerable<CategorySelectionModel>> GetAll();
    Task<PagingResult<Category>> GetAll(int pageIndex, int pageSize, string keyword, int? type);
    Task<string> Create(CategoryModel param);
    Task<CategoryModel> GetById(int id);
    Task<string> Update(CategoryModel param);
    Task DeleteAsync(int id);
    Task<List<WebCategoryViewModel>> GetCategoryForWeb();
    Task<bool> CheckExistInGoodsAsync(string code);
    Task ImportAsync(List<CategoryImport> categories);
    Task<string> ExportAsync(int type);
    Task<IEnumerable<CategorySelectionModel>> GetAll(List<int> types);
}
