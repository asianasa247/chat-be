using ManageEmployee.DataTransferObject.LedgerWarehouseModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.V2;
using ManageEmployee.Services.LedgerServices.V2;

namespace ManageEmployee.Services.Interfaces.Ledgers.V2;

public interface ILedgerV2Service
{
    Task<PagingResultLedger<LedgerV2Model>> GetPage(LedgerRequestModel request, int yearFilter);
    Task<LedgerDetailV2Model> GetByIdAsync(long id, int isInternal, int year);
    Task<TotalArisingForVoucherNumberModel> TotalArisingForVoucherNumber(string orginalVoucherNumber, int isInternal);
}
