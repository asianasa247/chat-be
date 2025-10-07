using ManageEmployee.Entities.LedgerEntities;

namespace ManageEmployee.Queues;

public interface ILedgerImportErrorQueue
{
    void Perform(List<Ledger> dataImports, int year);
}
