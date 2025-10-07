using Common.Constants;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.Services.Interfaces.Bills;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.BillServices;

public class BillTrackingForNotificationService : IBillTrackingForNotificationService
{
    private readonly ApplicationDbContext _context;
    public BillTrackingForNotificationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BillTrackingForCashierModel>> GetNotificationMessage(string userCode)
    {
        var result = await (from message in _context.BillTrackings
                            join bill in _context.Bills on message.BillId equals bill.Id
                            where !(message.Status == "Fail" || message.Status == "Cancel") && !message.IsRead && message.TranType != TranTypeConst.SendToStaff
                              && (message.BillId > 0 || message.UserCode == userCode)
                              && message.Type != BillTrackingTypeConst.WARNING_CHOOSE_GOODS_EXPIRATION

                            orderby message.Prioritize descending, message.Id descending
                            select new BillTrackingForCashierModel
                            {
                                Id = message.Id,
                                BillId = message.BillId,
                                UserCode = message.UserCode,
                                CustomerName = bill.CustomerName,
                                TranType = message.TranType,
                                Status = message.Status,
                                IsRead = message.IsRead,
                                IsImportant = message.IsImportant,
                                UserIdReceived = message.UserIdReceived,
                                CreatedDate = message.CreatedDate,
                                DisplayOrder = message.DisplayOrder,
                                Prioritize = message.Prioritize ?? 0,
                                BillNumber = bill.BillNumber,
                                TotalAmount = _context.BillDetails.Where(x => x.BillId == message.BillId).Sum(x => x.Quantity * (x.UnitPrice + x.TaxVAT) - x.DiscountPrice),
                            }).ToListAsync();
        return result.ConvertAll(x =>
        {
            x.PermisionShowButtons = PermisionShowButtonInNotification(x.TranType);
            return x;
        });
    }

    private string PermisionShowButtonInNotification(string tranType)
    {
        List<string> permission = new List<string>() { "ADD" };
        if (!tranType.Contains(TranTypeConst.Paid))
        {
            permission.Add("CANCEL");
        }
        if (tranType.Contains(TranTypeConst.Paid) && !tranType.Contains(TranTypeConst.SendToChef) && !tranType.Contains(TranTypeConst.Cooking) && !tranType.Contains(TranTypeConst.Cooked)
            || tranType.Contains(TranTypeConst.Paid) && tranType.Contains(TranTypeConst.Cooked)
            || tranType.Contains(TranTypeConst.Cancel))
        {
            permission.Add("HIDE");
        }
        if (tranType.Contains(TranTypeConst.Cooked))
        {
            permission.Add("ACCEPT");
        }
        return string.Join("-", permission);
    }
    public async Task<List<BillTrackingModel>> GetNotificationToStaffMessage(string userCode, int userId, string type)
    {
        return await (from message in _context.BillTrackings
                      where !(message.Status == "Fail" || message.Status == "Cancel")
                      && !message.IsRead && (message.BillId > 0 || message.UserCode == userCode || message.UserCode == userId.ToString())
                      && (string.IsNullOrEmpty(type) && message.Type == null && message.TranType == TranTypeConst.SendToStaff || message.Type == type && message.UserIdReceived == userId)

                      orderby message.Prioritize descending, message.Id
                      select new BillTrackingModel
                      {
                          Id = message.Id,
                          UserCode = message.UserCode,
                          CustomerName = message.CustomerName,
                          TranType = message.TranType,
                          Note = message.Note,
                          Status = message.Status,
                          IsRead = message.IsRead,
                          UserIdReceived = message.UserIdReceived,
                          CreatedDate = message.CreatedDate,
                          DisplayOrder = message.DisplayOrder,
                      }).ToListAsync();
    }
}
