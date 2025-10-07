namespace ManageEmployee.Handlers.ChartOfAccount;

public interface IChartOfAccountHandler
{
    void Handle(Entities.ChartOfAccountEntities.ChartOfAccount account, int year);
}