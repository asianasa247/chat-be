using ManageEmployee.DataTransferObject.MenuModels;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.Menus;

public interface IMenuService
{
    Task<IEnumerable<MenuViewModel>> GetAll(bool isParent);

    Task<PagingResult<MenuViewPagingModel>> GetAll(int pageIndex, int pageSize, string keyword, bool isParent,
        string codeParent, List<string> listRole, int userId, int? userRoleId, int? type = null);

    Task Create(MenuViewModel request);

    Task<MenuViewModel> GetById(int id, List<string> listRole, int userId);

    Task Update(MenuViewModel request, List<string> listRole, int userId);

    Task Delete(int id);

    Task<MenuCheckRole> CheckRole(string MenuCode, List<string> roleCodes);
    Task<List<MenuCheckRole>> GetListMenu(int userId);
}
