using ManageEmployee.DataTransferObject.BillModels;

namespace ManageEmployee.Services.Interfaces.Bills;

public interface IBillTrackingForNotificationService
{
    Task<List<BillTrackingForCashierModel>> GetNotificationMessage(string userCode);
    Task<List<BillTrackingModel>> GetNotificationToStaffMessage(string userCode, int userId, string type);
}
