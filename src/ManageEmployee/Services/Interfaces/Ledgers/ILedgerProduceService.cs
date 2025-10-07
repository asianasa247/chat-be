using ManageEmployee.DataTransferObject.LedgerModels;

namespace ManageEmployee.Services.Interfaces.Ledgers;

public interface ILedgerProduceService
{
    Task AddProduce(LedgerProduceModel request, int userId, int year);
}
