using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.Entities.BillEntities;

namespace ManageEmployee.Services.Interfaces.Bills;

public interface IBillDetailService
{
    Task<List<BillDetailViewPaging>> GetListByBillId(int billId);

    Task<List<BillDetail>> Create(List<BillDetailModel> requests, int year);

    Task<List<BillDetail>> UpdateNote(List<BillDetailNoteRequestModel> requests);

    Task Delete(int id);

    Task<List<BillDetailViewPaging>> GetListByBillIdForWareHouse(int billId, int year);

    Task RefundGoodsAsync(BillRefundModel billRefund, int billId, int year);

    Task<object> GetListBillDetailAndPromotion(int billId);
}