using ManageEmployee.Entities.LedgerEntities;

namespace ManageEmployee.Services.Interfaces.Ledgers;

public interface ILedgerUpdateChartOfAccountNameService
{
    Task<Ledger> UpdateChartOfAccountName(Ledger cEntity, int year);
}
