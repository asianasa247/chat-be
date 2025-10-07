using ManageEmployee.DataTransferObject.LedgerWarehouseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;

namespace ManageEmployee.Services.Interfaces.Ledgers;

public interface ILedgerWareHouseService
{
    Task Create(List<LedgerWarehouseCreate> requests, string typePay, int customerId, bool isPrintBill, int year);

    Task<PagingResult<LedgerWarehousePaging>> GetListHistory(LedgerWarehousesRequestPaging param);

    Task<LedgerWarehouseDetail> GetDetailHistory(int id);
}
