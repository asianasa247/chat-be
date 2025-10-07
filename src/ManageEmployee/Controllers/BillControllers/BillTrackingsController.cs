using Common.Constants;
using Common.Errors;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Bills;
using ManageEmployee.Services.Interfaces.DeskFloors;
using ManageEmployee.Extends;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.BillEntities;
using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers.BillControllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BillTrackingsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
    private readonly IDeskFloorService _deskFloorService;
    private readonly IBillTrackingForNotificationService _billTrackingForNotificationService;

    public BillTrackingsController(
        ApplicationDbContext context,
        IHubContext<BroadcastHub, IHubClient> hubContext,
        IDeskFloorService deskFloorService, IBillTrackingForNotificationService billTrackingForNotificationService)
    {
        _context = context;
        _hubContext = hubContext;
        _deskFloorService = deskFloorService;
        _billTrackingForNotificationService = billTrackingForNotificationService;
    }


    [Route("notificationcount")]
    [HttpGet]
    public async Task<ActionResult<BillTrackingCountResult>> GetNotificationCount()
    {
        var identityUser = HttpContext.GetIdentityUser();

        var count = (from message in _context.BillTrackings
                     where !(message.Status == "Fail" || message.Status == "Cancel") && !message.IsRead &&
                     message.TranType != TranTypeConst.SendToStaff
                      && (message.BillId > 0 || message.UserCode == identityUser.UserName)
                      && message.Type != BillTrackingTypeConst.WARNING_CHOOSE_GOODS_EXPIRATION
                     //&& message.TranType != "Paid"
                     select message).CountAsync();
        BillTrackingCountResult result = new BillTrackingCountResult
        {
            Count = await count
        };
        return result;
    }

    // GET: api/BillTracking/notificationcount
    [Route("notificationtostaffcount")]
    [HttpGet]
    public async Task<ActionResult<BillTrackingCountResult>> GetNotificationToStaffCount(string? type = "")
    {
        var identityUser = HttpContext.GetIdentityUser();

        var count = (from message in _context.BillTrackings
                     where
                     !(message.Status == "Fail" || message.Status == "Cancel") && !message.IsRead
                      && (message.BillId > 0 || message.UserCode == identityUser.UserName || message.UserCode == identityUser.Id.ToString())
                      && (string.IsNullOrEmpty(type) && message.Type == null && message.TranType == TranTypeConst.SendToStaff || message.Type == type && message.UserIdReceived == identityUser.Id)

                     select message).CountAsync();
        BillTrackingCountResult result = new BillTrackingCountResult
        {
            Count = await count
        };
        return result;
    }

    // GET: api/BillTracking/notificationresult
    [Route("notificationresult")]
    [HttpGet]
    public async Task<ActionResult<List<BillTrackingForCashierModel>>> GetNotificationMessage()
    {
        var identityUser = HttpContext.GetIdentityUser();
        var res = await _billTrackingForNotificationService.GetNotificationMessage(identityUser.UserName);
        return res;

    }

    // GET: api/BillTracking/notificationresult
    [Route("notificationtostaffresult")]
    [HttpGet]
    public async Task<ActionResult<List<BillTrackingModel>>> GetNotificationToStaffMessage(string? type = "")
    {
        var identityUser = HttpContext.GetIdentityUser();
        var res = await _billTrackingForNotificationService.GetNotificationToStaffMessage(identityUser.UserName, identityUser.Id, type);
        return res;
    }

    // GET: api/BillTracking/notification/{id}
    [Route("notification/{billId}")]
    [HttpGet]
    public async Task<ActionResult<BillTrackingModel>> GetNotificationMessageDetail(double billId)
    {
        var results = from message in _context.BillTrackings
                      where !(message.Status == "Fail" || message.Status == "Cancel")
                      && !message.IsRead && message.TranType != TranTypeConst.SendToStaff
                      && message.Type != BillTrackingTypeConst.WARNING_CHOOSE_GOODS_EXPIRATION

                      select new BillTrackingModel
                      {
                          Id = message.Id,
                          BillId = message.BillId,
                          UserCode = message.UserCode,
                          CustomerName = message.CustomerName,
                          TranType = message.TranType,
                          Status = message.Status,
                          IsRead = message.IsRead,
                          IsImportant = message.IsImportant,
                          UserIdReceived = message.UserIdReceived,
                          CreatedDate = message.CreatedDate,
                          DisplayOrder = message.DisplayOrder
                      };
        return await results.FirstOrDefaultAsync();
    }

    // GET: api/BillTracking/changestatus
    [Route("changestatus")]
    [HttpPut]
    public async Task<IActionResult> ChangeStatus(BillTrackingChangeStatusRequest requests)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();
            var billTracking = await _context.BillTrackings.FirstOrDefaultAsync(x => (x.Id == requests.Id && x.Status != "Cancel"
                                                                                    || requests.BillId > 0 && x.BillId == requests.BillId)
                                                                && x.Type != BillTrackingTypeConst.WARNING_CHOOSE_GOODS_EXPIRATION
                                                                                    );
            var bill = await _context.Bills.SingleOrDefaultAsync(x => x.Id == billTracking.BillId);
            if (billTracking == null)
            {
                return BadRequest(new { msg = "Bill not found" });
            }

            if (requests.CurrentTranType.Contains("-"))
            {
                requests.CurrentTranType = requests.CurrentTranType.Split('-')[0];
            }
            switch (requests.CurrentTranType)
            {
                case TranTypeConst.SendToCashier: billTracking.TranType = TranTypeConst.SendToChef; break;
                case TranTypeConst.SendToChef: billTracking.TranType = TranTypeConst.Cooking; break;
                case TranTypeConst.Cooking: billTracking.TranType = TranTypeConst.Cooked; break;
                case TranTypeConst.Cooked: billTracking.TranType = TranTypeConst.Paid; break;
                default: return BadRequest(new { msg = "Update Status fail" });
            }
            billTracking.UpdateAt = DateTime.Now;
            _context.BillTrackings.Update(billTracking);

            // update bill status
            if (bill.Status.Contains(TranTypeConst.Paid))
            {
                bill.Status = billTracking.TranType + "-" + TranTypeConst.Paid;
                billTracking.TranType = bill.Status;
            }
            _context.Bills.Update(bill);

            await _context.SaveChangesAsync();
            _context.Database.CommitTransaction();
            return Ok(new BaseResponseModel { Data = billTracking });
        }
        catch (Exception ex)
        {
            _context.Database.RollbackTransaction();
            throw new ErrorException(ex.Message);
        }
    }

    // GET: api/BillTracking/{id}
    [Route("{id}")]
    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(long id)
    {

        var billTracking = _context.BillTrackings.Find(id);
        if (billTracking != null)
        {
            var bill = _context.Bills.FirstOrDefault(X => X.Id == billTracking.BillId);
            if (bill != null)
            {
                bill.Status = "Paid";
                _context.Bills.Update(bill);
            }

            if (billTracking.IsImportant)
            {
                billTracking.IsRead = true;
                billTracking.Status = "Paid";
                billTracking.UpdateAt = DateTime.Now;

                _context.BillTrackings.Update(billTracking);

            }
            else
            {
                _context.BillTrackings.Remove(billTracking);
            }

            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.BroadcastMessage();
        }
        return Ok(new BaseResponseModel { Data = "OK" });
    }

    // DELETE: api/BillTracking/deletenotifications
    [HttpDelete]
    [Route("deletenotifications")]
    public async Task<IActionResult> DeleteNotifications()
    {
        List<BillTracking> listItem = await _context.BillTrackings.Where(x => x.Status == "Cancel" || x.TranType == "Paid").ToListAsync();
        var listBillId = listItem.Select(x => x.BillId).Distinct().ToList();
        var listDeskId = await _context.Bills.Where(X => listBillId.Contains(X.Id)).Select(x => x.DeskId).ToListAsync();
        foreach (var deskId in listDeskId)
        {
            _deskFloorService.UpdateDeskChoose(deskId, false);
        }
        _context.BillTrackings.RemoveRange(listItem);
        _context.SaveChanges();
        return Ok(new BaseResponseModel { Data = "OK" });
    }
    [HttpDelete]
    [Route("deletenotifications-work")]
    public async Task<IActionResult> DeleteNotificationWorks()
    {
        var identityUser = HttpContext.GetIdentityUser();
        string idUser = identityUser.Id.ToString();

        List<BillTracking> listItem = await _context.BillTrackings.Where(x => x.UserCode == idUser && x.BillId == 0
                                && x.Type != BillTrackingTypeConst.WARNING_CHOOSE_GOODS_EXPIRATION).ToListAsync();
        _context.BillTrackings.RemoveRange(listItem);
        await _context.SaveChangesAsync();
        return Ok(new BaseResponseModel { Data = "OK" });
    }

    [Route("changePriority")]
    [HttpGet]
    public async Task<IActionResult> changePriority(long Id)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();
            var billTracking = _context.BillTrackings.SingleOrDefault(x => x.Id == Id);
            if (billTracking is null)
                throw new ErrorException(ErrorMessage.DATA_IS_NOT_EXIST);

            int priority = _context.BillTrackings.Where(X => X.TranType.Contains(TranTypeConst.SendToChef)).Select(x => x.Prioritize ?? 0).OrderByDescending(x => x).FirstOrDefault();
            billTracking.Prioritize = priority + 1;
            _context.BillTrackings.Update(billTracking);
            await _context.SaveChangesAsync();
            _context.Database.CommitTransaction();
            return Ok(new BaseResponseModel { Data = billTracking });
        }
        catch (Exception ex)
        {
            _context.Database.RollbackTransaction();
            throw new ErrorException(ex.Message);
        }
    }
    [HttpGet("ReadMessage")]
    public async Task<IActionResult> ReadMessage(long id)
    {
        var billTracking = await _context.BillTrackings.FindAsync(id);
        if (billTracking != null)
        {
            billTracking.IsRead = true;
            billTracking.UpdateAt = DateTime.Now;

            _context.BillTrackings.Update(billTracking);
            await _context.SaveChangesAsync();
        }
        return Ok();
    }
}
