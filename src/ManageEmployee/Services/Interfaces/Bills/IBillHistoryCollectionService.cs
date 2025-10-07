using ManageEmployee.Entities.BillEntities;

namespace ManageEmployee.Services.Interfaces.Bills;

public interface IBillHistoryCollectionService
{
    Task<IEnumerable<BillHistoryCollection>> GetBillHistoryCollectionForBill(int billId);
    Task<string> Create(BillHistoryCollection request);
    Task<string> Update(BillHistoryCollection request);
}
