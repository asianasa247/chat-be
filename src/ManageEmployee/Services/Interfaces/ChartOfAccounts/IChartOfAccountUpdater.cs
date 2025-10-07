using ManageEmployee.Entities.ChartOfAccountEntities;

namespace ManageEmployee.Services.Interfaces.ChartOfAccounts;

public interface IChartOfAccountUpdater
{
    Task<string> Update(ChartOfAccount entity, int year);
}
