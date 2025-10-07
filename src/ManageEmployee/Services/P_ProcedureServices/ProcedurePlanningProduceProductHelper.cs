using Common.Constants;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.Entities.BillEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.ProduceProductEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Bills;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Services.Interfaces.ProduceProducts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.P_ProcedureServices;

public class ProcedurePlanningProduceProductHelper : IProcedurePlanningProduceProductHelper
{
    private readonly ApplicationDbContext _context;
    private readonly IBillService _billService;
    private readonly IBillDetailService _billDetailService;
    private readonly IProduceProductLedgerService _produceProductLedgerService;
    private readonly IBillPromotionService _billPromotionService;

    public ProcedurePlanningProduceProductHelper(ApplicationDbContext context,
        IBillService billService,
        IBillDetailService billDetailService,
        IProduceProductLedgerService produceProductLedgerService,
        IBillPromotionService billPromotionService)
    {
        _context = context;
        _billService = billService;
        _billDetailService = billDetailService;
        _produceProductLedgerService = produceProductLedgerService;
        _billPromotionService = billPromotionService;
    }

    public async Task<ProcedureStatusModelResponse> GetStatusAcceptForPlanningProduceProduct(int procedureStatusId, int planningProduceProductId, int userId, int year)
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
                case nameof(ProcedureOrderProduceProductConditionEnum.SendToCashier):
                    isChecking = true;
                    break;
                case nameof(ProcedureOrderProduceProductConditionEnum.PlanningWarehouse):
                    isChecking = true;
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
                    IsFinish = procedureStatusStep.IsFinish,
                    ProcedureConditionCode = procedureStatusStep.ProcedureConditionCode,
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

    public async Task<bool> SendToCashierAsync(IEnumerable<PlanningProduceProductDetail> planningProduceProductDetails, int planningProduceProductId, int userId, string billStatus, int year, string produceCode)
    {
        var goodIds = planningProduceProductDetails.Select(x => x.GoodsId).ToList();
        var goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();

        var customerIds = planningProduceProductDetails.Select(x => x.CustomerId).Distinct().ToList();
        foreach (var customerId in customerIds)
        {
            var planningProduceProductDetailBills = planningProduceProductDetails.Where(x => x.CustomerId == customerId).ToList();
            // create a new bill
            var billNumber = await _billService.GetBillTrackingOrder("BSN");

            var billModel = new BillModel
            {
                UserCode = "",
                TotalAmount = planningProduceProductDetailBills.Sum(x => x.Quantity * x.UnitPrice),
                QuantityCustomer = 1,
                BillNumber = billNumber.Item2,
                CustomerId = customerId,
                UserCreated = userId,
                IsPayment = true,
                IsPrintBill = true,
                DisplayOrder = Convert.ToInt32(billNumber.Item1),
                Status = billStatus,
                Type = "BSN",
                TypePay = "CN"
            };
            var billPromotion = await _billPromotionService.GetBillPromotion(planningProduceProductId, nameof(PlanningProduceProduct), customerId: customerId);


            var bill = await _billService.Create(billModel);
            await _billPromotionService.Copy(billPromotion, bill.Id, nameof(Bill));

            // create bill details
            var billDetails = planningProduceProductDetailBills.Select(x => new BillDetailModel
            {
                BillId = bill.Id,
                GoodsId = x.GoodsId,
                Quantity = Convert.ToInt32(x.Quantity),
                UnitPrice = x.UnitPrice,
                TaxVAT = x.TaxVAT,
                DeliveryCode = $"{produceCode} - {x.DeliveryCode}" 
            }).ToList();
            await _billDetailService.Create(billDetails, year);
        }
        return true;
    }

}