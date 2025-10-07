using ManageEmployee.Entities.ChartOfAccountEntities;

namespace ManageEmployee.Services.Interfaces.ChartOfAccounts;

public interface IChartOfAccountCalculateBalancer
{
    Task CalculateBalance(ChartOfAccount account, int year, ChartOfAccount parent = null);
}
