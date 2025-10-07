using ManageEmployee.Entities;
using ManageEmployee.Entities.ChartOfAccountEntities;

namespace ManageEmployee.Services.Interfaces.ChartOfAccounts;

public interface ICharOfAccountSyncService
{
    Task<(List<ChartOfAccount> accountAdds, List<ChartOfAccount> accountUpdates)> SyncAccountGroupAsync(ChartOfAccount entity, string grandParentRef, string parentCode, List<Warehouse> warehouses, ChartOfAccount accountParent, int year);
    Task<(List<ChartOfAccount> accountAdds, List<ChartOfAccount> accountUpdates)> SyncListAccountGroupAsync(List<ChartOfAccount> entities, string parentCode, List<Warehouse> warehouses, List<ChartOfAccount> listAccount, int year);
}
