using ManageEmployee.DataTransferObject.V2;
using ManageEmployee.DataTransferObject.V3;

namespace ManageEmployee.Services.Interfaces.Ledgers.V3;

public interface ILedgerV3Service
{
    Task<List<LedgerDetailV3Model>> GetLedgerById(long ledgerId, int year);

    Task UpdateAsync(List<LedgerV3UpdateModel> requests, int year, bool isReverse =false);
}
