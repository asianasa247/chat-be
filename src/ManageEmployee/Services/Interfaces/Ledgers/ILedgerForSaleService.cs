using ManageEmployee.Entities.BillEntities;
using ManageEmployee.Entities.ProduceProductEntities;

namespace ManageEmployee.Services.Interfaces.Ledgers;

public interface ILedgerForSaleService
{
    Task AddLedgerBill(List<BillDetail> listBillDetail, int year);
    Task AddLedgerProduceProduct(List<ProduceProductDetail> produceProductDetails, string typePayLedger, int year, int? responsibleUserId);
}
