using ManageEmployee.DataTransferObject.SelectModels;
using ManageEmployee.Entities.ChartOfAccountEntities;

namespace ManageEmployee.Services.Interfaces.ChartOfAccounts;

public interface IChartOfAccountGroupLinkService
{
    IEnumerable<ChartOfAccountGroupLink> GetAll(int year);

    IEnumerable<ChartOfAccountGroupLink> GetAll(int currentPage, int pageSize, int year);

    ChartOfAccountGroupLink GetById(int id);

    ChartOfAccountGroupLink Create(ChartOfAccountGroupLink param, int year);

    void Update(ChartOfAccountGroupLink param, int year);

    void Delete(int id);

    Task<List<SelectListModel>> GetAllAccountGroupLinks(int year);
}
