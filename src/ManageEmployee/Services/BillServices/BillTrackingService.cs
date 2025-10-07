using Common.Constants;
using Common.Errors;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Hubs;
using ManageEmployee.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Services.Interfaces.Bills;
using ManageEmployee.Services.Interfaces.Ledgers;
using ManageEmployee.Services.Interfaces.DeskFloors;
using ManageEmployee.Entities.BillEntities;
using ManageEmployee.DataTransferObject.BillModels;

namespace ManageEmployee.Services.BillServices;


public class BillTrackingService : IBillTrackingService
{
    private readonly ApplicationDbContext _context;
    private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
    private readonly IBillDetailService _billDetailService;
    private readonly ILedgerService _ledgerService;
    private readonly IDeskFloorService _deskFloorService;

    public BillTrackingService(
        ApplicationDbContext context,
        IHubContext<BroadcastHub, IHubClient> hubContext,
        IBillDetailService billDetailService,
        ILedgerService ledgerService,
        IDeskFloorService deskFloorService
    )
    {
        _context = context;
        _hubContext = hubContext;
        _billDetailService = billDetailService;
        _ledgerService = ledgerService;
        _deskFloorService = deskFloorService;
    }

    public async Task<Bill> onCompleteBill(int id, BillModel bill, int year)
    {
        BillTracking billTracking = await _context.BillTrackings.FirstOrDefaultAsync(x => x.BillId == id);
        if (billTracking == null)
        {
            throw new ErrorException(ErrorMessage.BILL_IS_NOT_FOUND);
        }
        if (billTracking.TranType.Contains(TranTypeConst.Paid))
            throw new ErrorException(ErrorMessage.BILL_IS_PAYED);

        if (billTracking.TranType == TranTypeConst.SendToCashier || billTracking.TranType == TranTypeConst.UserReceived)
            billTracking.TranType = TranTypeConst.Paid;
        else if (!billTracking.TranType.Contains(TranTypeConst.Paid))
            billTracking.TranType = billTracking.TranType + "-" + TranTypeConst.Paid;
        billTracking.UpdateAt = DateTime.Now;

        _context.BillTrackings.Update(billTracking);
        if (billTracking.TranType == TranTypeConst.Paid)
            _deskFloorService.UpdateDeskChoose(bill.DeskId, false);

        var billData = Update(bill, billTracking.TranType);
        List<BillDetail> listBillDetailDel = await _context.BillDetails.Where(x => x.BillId == id).ToListAsync();
        _context.BillDetails.RemoveRange(listBillDetailDel);

        var listLedger = await _context.Ledgers.Where(x => x.BillId == id).ToListAsync();
        _context.Ledgers.RemoveRange(listLedger);
        // update amount in account
        //delete ledger

        var ledgerDeleteIds = listLedger.Select(X => X.Id);
        if (ledgerDeleteIds.Any())
        {
            var ledgerDeleteIdString = string.Join(",", ledgerDeleteIds);
            await _ledgerService.Delete(ledgerDeleteIdString, 1, year);
        }


        List<BillDetailModel> listBillDetail_requets = bill.Products;

        await _billDetailService.Create(listBillDetail_requets, year);
        var listBillTracking = await _context.BillTrackings.Where(x => x.TranType == TranTypeConst.Paid).OrderBy(x => x.UpdateAt).ToListAsync();
        if (listBillTracking.Count > 3)
        {
            listBillTracking.RemoveRange(listBillTracking.Count - 3, 3);
            _context.BillTrackings.RemoveRange(listBillTracking);
        }


        try
        {
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.BroadcastMessage();
            return billData;
        }
        catch (Exception ex)
        {
            throw new ErrorException(ex.Message);
        }
    }

    public async Task<BillTracking> onSendToChefBill(int id)
    {
        BillTracking billTracking = await _context.BillTrackings.FirstOrDefaultAsync(x => x.BillId == id && x.Type != BillTrackingTypeConst.WARNING_CHOOSE_GOODS_EXPIRATION
);
        if (billTracking == null)
        {
            return null;
        }
        billTracking.TranType = TranTypeConst.SendToChef;
        billTracking.UpdateAt = DateTime.Now;

        _context.BillTrackings.Update(billTracking);
        await _context.SaveChangesAsync();
        return billTracking;
    }

    public async Task<BillTracking> receivedBill(int id, int userId)
    {
        BillTracking billTracking = await _context.BillTrackings.Where(x => x.BillId == id && x.Type != BillTrackingTypeConst.WARNING_CHOOSE_GOODS_EXPIRATION).FirstOrDefaultAsync();
        if (billTracking == null)
        {
            return null;
        }
        billTracking.UserIdReceived = userId;
        if (billTracking.TranType.Contains(TranTypeConst.Paid))
            billTracking.TranType = TranTypeConst.UserReceived + "-" + TranTypeConst.Paid;
        else
            billTracking.TranType = TranTypeConst.UserReceived;
        billTracking.UpdateAt = DateTime.Now;

        _context.BillTrackings.Update(billTracking);
        await _context.SaveChangesAsync();
        return billTracking;
    }

    public async Task Create(BillModel billModel, Bill bill)
    {
        var billTracking = new BillTracking()
        {
            BillId = bill.Id,
            CustomerName = bill.CustomerName,
            UserCode = bill.UserCode,
            Status = "Success",
            DisplayOrder = bill.DisplayOrder.GetValueOrDefault(1),
            TranType = billModel.IsPayment
                    ? TranTypeConst.Paid
                    : billModel.UserType == "cashier" ? TranTypeConst.SendToChef : TranTypeConst.SendToCashier,
        };

        if (bill.IsPriority)
        {
            billTracking.Prioritize = await GetNextPriorityNumber("SendToChef");
        }

        await _context.BillTrackings.AddAsync(billTracking);
        await _context.SaveChangesAsync();

        _context.Entry(billTracking).State = EntityState.Detached;

        await _hubContext.Clients.All.BroadcastMessage();
    }

    private Bill Update(BillModel requests, string status)
    {
        var bill = _context.Bills.SingleOrDefault(x => x.Id == requests.Id && !x.IsDeleted);
        if (bill == null)
        {
            throw new ErrorException(ErrorMessage.DATA_IS_NOT_EXIST);
        }
        bill.CustomerId = requests.CustomerId;
        bill.CustomerName = requests.CustomerName;
        bill.QuantityCustomer = requests.QuantityCustomer;
        bill.TotalAmount = requests.TotalAmount;
        bill.DiscountPrice = requests.DiscountPrice;
        bill.DiscountType = requests.DiscountType;
        bill.UserCode = requests.UserCode;
        bill.DeskId = requests.DeskId;
        bill.FloorId = requests.FloorId;
        bill.AmountReceivedByCus = requests.AmountReceivedByCus;
        bill.AmountSendToCus = requests.AmountSendToCus;
        bill.Note = requests.Note;
        bill.UpdatedDate = DateTime.Now;
        bill.TypePay = requests.TypePay;
        bill.Status = status;
        bill.Vat = requests.Vat;
        bill.VatCode = requests.VatCode;
        bill.VatRate = requests.VatRate;
        _context.Bills.Update(bill);
        return bill;

    }
    private async Task<int> GetNextPriorityNumber(string tranType)
    {
        int nextPriority = 0;
        var priority = await _context.BillTrackings.Where(X => X.TranType.Equals(tranType)).Select(x => x.Prioritize.GetValueOrDefault()).ToListAsync();

        if (priority.Any())
            nextPriority = priority.Max();

        return nextPriority + 1;
    }

    public async Task<BillTracking> CancelBill(int id, int userId)
    {
        var billTracking = await _context.BillTrackings.FirstOrDefaultAsync(x => x.BillId == id);
        if (billTracking == null)
        {
            throw new ErrorException(ErrorMessage.DATA_IS_EMPTY);
        }

        billTracking.UserIdReceived = userId;

        if (billTracking.TranType.Contains(TranTypeConst.Paid))
            throw new ErrorException(ErrorMessage.BILL_IS_PAYED);

        billTracking.TranType = TranTypeConst.Cancel;
        billTracking.Status = TranTypeConst.Cancel;
        billTracking.UpdateAt = DateTime.Now;

        _context.BillTrackings.Update(billTracking);

        var bill = await _context.Bills.FindAsync(id);
        if (bill == null)
        {
            throw new ErrorException(ErrorMessage.NOT_DECLARE_INVOICE);
        }
        bill.Status = TranTypeConst.Cancel;
        _context.Bills.Update(bill);

        await _context.SaveChangesAsync();
        return billTracking;
    }
}