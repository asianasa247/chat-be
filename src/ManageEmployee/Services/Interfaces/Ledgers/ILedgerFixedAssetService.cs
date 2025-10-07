using ManageEmployee.Entities.LedgerEntities;

namespace ManageEmployee.Services.Interfaces.Ledgers;

public interface ILedgerFixedAssetService
{
    Task CreateAsync(LedgerFixedAsset form);
    Task UpdateAsync(Ledger ledger, int year);
}
