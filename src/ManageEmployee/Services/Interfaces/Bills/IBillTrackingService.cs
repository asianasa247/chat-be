using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.Entities.BillEntities;

namespace ManageEmployee.Services.Interfaces.Bills;

public interface IBillTrackingService
{
    Task<BillTracking> onSendToChefBill(int id);
    Task<Bill> onCompleteBill(int id, BillModel bill, int year);
    Task<BillTracking> receivedBill(int id, int userId);
    Task Create(BillModel billModel, Bill bill);
    Task<BillTracking> CancelBill(int id, int userId);
}