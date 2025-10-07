using ManageEmployee.Entities.ChartOfAccountEntities;

namespace ManageEmployee.Services.Interfaces.ChartOfAccounts;

public interface INameCodeAccountChanger
{
    Task ChangeNameCodeAccount(ChartOfAccount form, ChartOfAccount oldAccount, int year);
}
