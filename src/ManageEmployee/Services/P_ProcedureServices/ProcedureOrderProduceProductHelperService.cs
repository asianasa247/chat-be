using Common.Constants;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Bills;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Services.Interfaces.ProduceProducts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.P_ProcedureServices;

public class ProcedureOrderProduceProductHelperService : IProcedureOrderProduceProductHelperService
{
    private readonly ApplicationDbContext _context;
    private readonly IBillService _billService;
    private readonly IBillDetailService _billDetailService;
    private readonly IProduceProductLedgerService _produceProductLedgerService;

    public ProcedureOrderProduceProductHelperService(ApplicationDbContext context,
        IBillService billService,
        IBillDetailService billDetailService,
        IProduceProductLedgerService produceProductLedgerService)
    {
        _context = context;
        _billService = billService;
        _billDetailService = billDetailService;
        _produceProductLedgerService = produceProductLedgerService;
    }

    public async Task<ProcedureStatusModelResponse> GetStatusAcceptForOrderProduceProduct(int procedureStatusId, int orderProduceProductId, int userId, int year)
    {
        var procedureStatusSteps = await _context.P_ProcedureStatusSteps
                        .Where(x => x.ProcedureStatusIdFrom == procedureStatusId).ToListAsync();
        if (!procedureStatusSteps.Any())
        {
            return new ProcedureStatusModelResponse();
        }

        var isChecking = false;
        var procedureStatusStepHaveConditions = procedureStatusSteps.Where(x => x.ProcedureConditionId != null && x.ProcedureConditionId > 0);
        foreach (var procedureStatusStep in procedureStatusStepHaveConditions)
        {
            var procedureStatus = await _context.P_ProcedureStatus.FirstOrDefaultAsync(x => x.Id == procedureStatusStep.ProcedureStatusIdTo);
            if (procedureStatus == null)
                continue;

            switch (procedureStatusStep.ProcedureConditionCode)
            {
                case nameof(ProcedureOrderProduceProductConditionEnum.PriceLower):
                    isChecking = await CheckOrderProduceProductPriceLower(orderProduceProductId);
                    break;

                case nameof(ProcedureOrderProduceProductConditionEnum.SendToCashier):
                    isChecking = await SendToCashier(orderProduceProductId, userId, TranTypeConst.SendToCashier, year);
                    break;

                case nameof(ProcedureOrderProduceProductConditionEnum.SendToWarehouse):
                    isChecking = await SendToCashier(orderProduceProductId, userId, TranTypeConst.Waiting, year);
                    break;

                case nameof(ProcedureOrderProduceProductConditionEnum.Special):
                    isChecking = await CheckSpecialOrder(orderProduceProductId);
                    break;

                case nameof(ProcedureOrderProduceProductConditionEnum.LedgerDebit):
                    isChecking = await _produceProductLedgerService.AddLedgerDebitFromOrderProduct(orderProduceProductId, year);
                    break;

                default:
                    break;
            }

            if (isChecking)
            {
                return new ProcedureStatusModelResponse()
                {
                    Id = procedureStatus.Id,
                    P_StatusName = procedureStatus.Name,
                    IsFinish = procedureStatusStep.IsFinish
                };
            }
        }

        var procedureStatusStepNotHaveCondition = procedureStatusSteps.FirstOrDefault(x => x.ProcedureConditionId == null || x.ProcedureConditionId == 0);
        if (procedureStatusStepNotHaveCondition is null)
        {
            throw new ErrorException("Không có trạng thái kế tiếp thỏa mãn điều kiện");
        }

        var procedureStatusNotHaveCondition = await _context.P_ProcedureStatus.FirstOrDefaultAsync(x => x.Id == procedureStatusStepNotHaveCondition.ProcedureStatusIdTo);
        if (procedureStatusNotHaveCondition == null)
            return new ProcedureStatusModelResponse();
        return new ProcedureStatusModelResponse()
        {
            Id = procedureStatusNotHaveCondition.Id,
            P_StatusName = procedureStatusNotHaveCondition.Name,
            IsFinish = procedureStatusStepNotHaveCondition.IsFinish
        };
    }

    private async Task<bool> CheckOrderProduceProductPriceLower(int orderProduceProductId)
    {
        var orderProduceProduct = await _context.OrderProduceProducts.FindAsync(orderProduceProductId);
        if (orderProduceProduct is null)
        {
            return true;
        }
        var orderProduceProductDetails = await _context.OrderProduceProductDetails.Where(x => x.OrderProduceProductId == orderProduceProductId).ToListAsync();
        var goodIds = orderProduceProductDetails.Select(x => x.GoodsId).ToList();
        var goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();

        foreach (var orderProduceProductDetail in orderProduceProductDetails)
        {
            var good = goods.FirstOrDefault(X => X.Id == orderProduceProductDetail.GoodsId);
            if (good is null)
                continue;
            if (orderProduceProductDetail.UnitPrice < good.SalePrice)
                return true;
        }
        return false;
    }

    private async Task<bool> SendToCashier(int orderProduceProductId, int userId, string billStatus, int year)
    {
        var orderProduceProduct = await _context.OrderProduceProducts.FindAsync(orderProduceProductId);
        if (orderProduceProduct is null)
        {
            return true;
        }
        var orderProduceProductDetails = await _context.OrderProduceProductDetails.Where(x => x.OrderProduceProductId == orderProduceProductId).ToListAsync();
        var goodIds = orderProduceProductDetails.Select(x => x.GoodsId).ToList();
        var goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();

        var bill = await _context.Bills.FirstOrDefaultAsync(x => x.OrderProduceProductId == orderProduceProductId);
        if (bill != null)
        {
            bill.Status = billStatus;
            _context.Bills.Update(bill);

            var billTracking = await _context.BillTrackings.FirstOrDefaultAsync(x => x.BillId == bill.Id);
            if (billTracking != null)
            {
                billTracking.Status = billStatus;
                _context.BillTrackings.Update(billTracking);
            }

            await _context.SaveChangesAsync();
            return true;
        }
        // create a new bill
        var billNumber = await _billService.GetBillTrackingOrder("BSN");

        var billModel = new BillModel
        {
            UserCode = "",
            TotalAmount = orderProduceProductDetails.Sum(x => x.QuantityReal * x.UnitPrice),
            QuantityCustomer = 1,
            BillNumber = billNumber.Item2,
            CustomerId = orderProduceProduct.CustomerId,
            UserCreated = userId,
            IsPayment = false,
            IsPrintBill = true,
            DisplayOrder = Convert.ToInt32(billNumber.Item1),
            Status = billStatus,
            Type = "BSN"
        };
        bill = await _billService.Create(billModel, orderProduceProductId);
        // create bill details
        var billDetails = orderProduceProductDetails.Select(x => new BillDetailModel
        {
            BillId = bill.Id,
            GoodsId = x.GoodsId,
            Quantity = Convert.ToInt32(x.QuantityReal),
            UnitPrice = x.UnitPrice,
            TaxVAT = x.TaxVAT,
        }).ToList();
        await _billDetailService.Create(billDetails, year);

        return true;
    }

    private async Task<bool> CheckSpecialOrder(int orderProduceProductId)
    {
        var orderProduceProduct = await _context.OrderProduceProducts.FindAsync(orderProduceProductId);
        if (orderProduceProduct is null)
        {
            return true;
        }

        return orderProduceProduct.IsSpecialOrder;
    }
}