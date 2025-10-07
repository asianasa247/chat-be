using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.MenuEntities;

namespace ManageEmployee.Services.Interfaces.Menus;

public interface IMenuKpiService
{
    IEnumerable<MenuKpi> GetAll();
    Task<PagingResult<MenuKpi>> GetAll(int pageIndex, int pageSize, string keyword, int type);
    Task<string> Create(MenuKpi request);
    MenuKpi GetById(int id);
    Task<string> Update(MenuKpi request);
    string Delete(int id);
}
