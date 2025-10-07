namespace ManageEmployee.Services.Interfaces.ChartOfAccounts;

public interface IChartOfAccountDeleter
{
    Task Delete(long id, int year);

}
