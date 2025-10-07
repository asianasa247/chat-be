using ManageEmployee.Entities.ChartOfAccountEntities;

namespace ManageEmployee.Services.Interfaces.ChartOfAccounts;

public interface IChartOfAccountGroupService
{
    IEnumerable<ChartOfAccountGroup> GetAll(int year);

    IEnumerable<ChartOfAccountGroup> GetAll(int currentPage, int pageSize, int year);

    ChartOfAccountGroup GetById(int id);

    ChartOfAccountGroup Create(ChartOfAccountGroup param, int year);

    void Update(ChartOfAccountGroup param);

    void Delete(int id);
}
