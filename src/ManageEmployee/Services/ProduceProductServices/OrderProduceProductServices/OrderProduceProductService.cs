using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.ProduceProductModels;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.ProduceProductEntities;
using ManageEmployee.Extends;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Bills;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Services.Interfaces.ProduceProducts.OrderProduceProducts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.ProduceProductServices.OrderProduceProductServices;

public class OrderProduceProductService : IOrderProduceProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;
    private readonly IProcedureOrderProduceProductHelperService _procedureOrderProduceProductHelperService;
    private readonly IBillPromotionService _billPromotionService;
    public OrderProduceProductService(ApplicationDbContext context, IMapper mapper,
        IProcedureHelperService procedureHelperService,
        IProcedureOrderProduceProductHelperService procedureOrderProduceProductHelperService,
        IBillPromotionService billPromotionService)
    {
        _context = context;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
        _procedureOrderProduceProductHelperService = procedureOrderProduceProductHelperService;
        _billPromotionService = billPromotionService;
    }

    public async Task<PagingResult<OrderProduceProductPagingModel>> GetPaging(OrderProduceProductPagingRequestModel param, int userId)
    {
        var query = _context.OrderProduceProducts.Where(x => x.Id > 0);

        if (!string.IsNullOrEmpty(param.SearchText))
        {
            query = query.Where(x => x.ProcedureNumber.Contains(param.SearchText));
        }
        var startStatus = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.ORDER_PRODUCE_PRODUCT));
        var statusIds = await _procedureHelperService.GetProcedureStatusIds(userId, param.StatusTab, nameof(ProcedureEnum.ORDER_PRODUCE_PRODUCT));
        query = query.Where(x => statusIds.Contains(x.ProcedureStatusId));
        var lstId = query.Select(x => x.Id);
        var queryDetail = _context.OrderProduceProductDetails.Where(x => lstId.Contains(x.OrderProduceProductId));
        if (param.StatusTab == ProduceProductStatusTab.Approved)
        {
            query = query
            .Join(_context.ProcedureLogs,
                    b => b.Id,
                    d => d.ProcedureId,
                    (b, d) => new
                    {
                        procedure = b,
                        log = d
                    })
            .Where(x => x.log.ProcedureCode == nameof(ProcedureEnum.ORDER_PRODUCE_PRODUCT) && (x.log.UserId == userId || x.procedure.UserId == userId)
                        && x.log.NotAcceptCount == 0
                        && !x.procedure.IsFinished)
            .Select(x => x.procedure).Distinct();
        }
        if (param.StatusTab == ProduceProductStatusTab.Cancel || param.StatusTab == ProduceProductStatusTab.Part)
        {
            query = query.Where(x => x.IsCanceled && param.StatusTab == ProduceProductStatusTab.Cancel ?
                queryDetail.Where(q => q.OrderProduceProductId == x.Id && q.IsDone).Sum(x => x.QuantityDelivered) == 0
                :
                queryDetail.Any(q => q.OrderProduceProductId == x.Id && q.QuantityDelivered > 0) && x.IsCanceled
            );
        }
        else
        {
            query = query.Where(x => !x.IsCanceled);
        }

        if (param.StatusTab == ProduceProductStatusTab.Done)
        {
            query = query.Where(x => x.IsFinished);
        }

        if (param.StatusTab == ProduceProductStatusTab.Finish)
        {
            query = query.Where(x => x.IsDone && queryDetail.Where(q => q.OrderProduceProductId == x.Id).All(q => q.IsDone && q.QuantityDelivered == q.QuantityRequired));
        }
        query = query.QueryDate(param);

        var data = await query.OrderByDescending(x => x.CreatedAt).Skip(param.PageSize * param.Page).Take(param.PageSize).ToListAsync();
        var totalItem = await query.CountAsync();
        var customerIds = data.Select(x => x.CustomerId).Distinct();
        var customers = await _context.Customers.Where(x => customerIds.Contains(x.Id)).ToListAsync();

        var userIds = data.Select(x => x.UserCreated).Distinct();
        var users = await _context.Users.Where(x => userIds.Contains(x.Id)).ToListAsync();

        var listOut = new List<OrderProduceProductPagingModel>();

        foreach (var item in data)
        {
            var itemOut = _mapper.Map<OrderProduceProductPagingModel>(item);
            itemOut.Quantity = await queryDetail.Where(x => x.OrderProduceProductId == item.Id).SumAsync(x => x.QuantityRequired);
            itemOut.TotalAmount = await queryDetail.Where(x => x.OrderProduceProductId == item.Id).SumAsync(x => x.QuantityRequired * x.UnitPrice) -
                await _billPromotionService.GetTotalAmountPromotion(item.Id, nameof(OrderProduceProduct));
            itemOut.QuantityDelivered = await queryDetail.Where(x => x.OrderProduceProductId == item.Id).SumAsync(x => x.QuantityDelivered);
            itemOut.QuantityInProgress = await queryDetail.Where(x => x.OrderProduceProductId == item.Id).SumAsync(x => x.QuantityInProgress);

            itemOut.CustomerName = customers.FirstOrDefault(x => x.Id == item.CustomerId)?.Name;
            itemOut.CustomerCode = customers.FirstOrDefault(x => x.Id == item.CustomerId)?.Code;
            itemOut.ShoulDelete = !itemOut.IsFinished && userId == item.UserCreated && param.StatusTab == ProduceProductStatusTab.Pending
                                    && itemOut.ProcedureStatusName == startStatus.P_StatusName;
            itemOut.ShoulNotAccept = !itemOut.IsFinished && param.StatusTab == ProduceProductStatusTab.Pending && itemOut.ProcedureStatusName != startStatus.P_StatusName;

            itemOut.UserCreatedCode = users.FirstOrDefault(x => x.Id == item.UserCreated)?.Username;
            itemOut.UserCreatedName = users.FirstOrDefault(x => x.Id == item.UserCreated)?.FullName;
            if (itemOut.IsFinished)
            {
                itemOut.ProcedureNumber = itemOut.Code;
            }
            listOut.Add(itemOut);
        }

        return new PagingResult<OrderProduceProductPagingModel>
        {
            Data = listOut,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task<OrderProduceProductModel> GetDetail(int id, int year)
    {
        var item = await _context.OrderProduceProducts.Where(x => x.Id == id).Select(x => _mapper.Map<OrderProduceProductModel>(x)).FirstOrDefaultAsync();
        if (item is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }

        item.Items = await _context.OrderProduceProductDetails.Where(x => x.OrderProduceProductId == id).Select(x => _mapper.Map<OrderProduceProductDetailModel>(x)).ToListAsync();
        item.BillPromotions = await _billPromotionService.Get(id, nameof(OrderProduceProduct));

        var goodIds = item.Items.Select(x => x.GoodsId).ToList();
        var goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();
        var goodsDetails = await _context.GoodDetails.Where(x => goodIds.Contains(x.GoodID ?? 0)).ToListAsync();

        var listStorege = await _context.GetChartOfAccount(year).Where(x => (x.Classification == 2 || x.Classification == 3) && !x.HasChild).ToListAsync();

        foreach (var itemGood in item.Items)
        {
            var good = goods.FirstOrDefault(x => x.Id == itemGood.GoodsId);
            if (good != null)
            {
                itemGood.GoodsCode = GoodNameGetter.GetCodeFromGood(good);
                itemGood.GoodsName = GoodNameGetter.GetNameFromGood(good);
                if (good.GoodsType == nameof(GoodsTypeEnum.DM))
                {
                    var goodsDetail = goodsDetails.Where(x => x.GoodID == itemGood.GoodsId).ToList();

                    itemGood.GoodsQuotes = goodsDetail.ConvertAll(x =>
                    {
                        var detail = new GoodsQuotaForOrderProduceProductDetailModel
                        {
                            GoodsCode = GoodNameGetter.GetCodeFromGoodDetail(x),
                            GoodsName = GoodNameGetter.GetNameFromGoodDetail(x),
                            UnitPrice = x.UnitPrice ?? 0,
                            QuantityInProgress = itemGood.QuantityInProgress,
                            QuantityDelivered = itemGood.QuantityDelivered,
                            QuantityRequired = itemGood.QuantityRequired * Convert.ToDouble(x.Quantity ?? 0),
                            QuantityReal = itemGood.QuantityReal * Convert.ToDouble(x.Quantity ?? 0),
                            QuantityQuote = Convert.ToDouble(x.Quantity ?? 0)
                        };

                        ChartOfAccount storegeDetail;
                        if (!string.IsNullOrEmpty(x.Detail2))
                        {
                            string parentRef = x.Account + ":" + x.Detail1;
                            storegeDetail = listStorege.Find(y => y.Code == x.Detail2 && y.ParentRef == parentRef &&
                                    (string.IsNullOrEmpty(x.Warehouse) || y.WarehouseCode == x.Warehouse));
                        }
                        else if (!string.IsNullOrEmpty(x.Detail1))
                            storegeDetail = listStorege.Find(y => y.Code == x.Detail1 && y.ParentRef == x.Account &&
                            (string.IsNullOrEmpty(x.Warehouse) || y.WarehouseCode == x.Warehouse));
                        else
                            storegeDetail = listStorege.Find(y => y.Code == x.Account);

                        if (storegeDetail != null)
                        {
                            detail.QuantityStock = (storegeDetail.OpeningStockQuantityNB ?? 0) + (storegeDetail.ArisingStockQuantityNB ?? 0);
                        }
                        return detail;
                    });
                }
                ChartOfAccount storege;
                if (!string.IsNullOrEmpty(good.Detail2))
                {
                    string parentRef = good.Account + ":" + good.Detail1;
                    storege = listStorege.Find(x => x.Code == good.Detail2 && x.ParentRef == parentRef &&
                            (string.IsNullOrEmpty(good.Warehouse) || x.WarehouseCode == good.Warehouse));
                }
                else if (!string.IsNullOrEmpty(good.Detail1))
                    storege = listStorege.Find(x => x.Code == good.Detail1 && x.ParentRef == good.Account &&
                    (string.IsNullOrEmpty(good.Warehouse) || x.WarehouseCode == good.Warehouse));
                else
                    storege = listStorege.Find(x => x.Code == good.Account);

                if (storege != null)
                {
                    itemGood.QuantityStock = (storege.OpeningStockQuantityNB ?? 0) + (storege.ArisingStockQuantityNB ?? 0);
                }
            }
        }
        return item;
    }

    public async Task<OrderProduceProduct> Create(OrderProduceProductCreateModel form, int userId)
    {
        var produce = new OrderProduceProduct
        {
            CustomerId = form.CustomerId,
            UserId = userId,
            Date = DateTime.Now,
            Note = form.Note,
            CreatedAt = DateTime.Now,
            UserCreated = userId,
            UserUpdated = userId,
            UpdatedAt = DateTime.Now,
            IsSpecialOrder = false,
        };

        if (string.IsNullOrEmpty(produce.ProcedureNumber))
        {
            produce.ProcedureNumber = await GetProcedureNumber();
        }

        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.ORDER_PRODUCE_PRODUCT));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        await _context.OrderProduceProducts.AddAsync(produce);
        await _context.SaveChangesAsync();

        var details = form.Items.ConvertAll(x => new OrderProduceProductDetail
        {
            OrderProduceProductId = produce.Id,
            GoodsId = x.GoodsId,
            QuantityRequired = x.Quantity,
            QuantityReal = x.Quantity,
            UnitPrice = x.UnitPrice,
            TaxVAT = x.TaxVAT,
            DiscountPrice = x.DiscountPrice,
        });
        await _context.OrderProduceProductDetails.AddRangeAsync(details);

        // add promotions
        await _billPromotionService.Create(form.BillPromotions, produce.Id, nameof(OrderProduceProduct));

        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.ORDER_PRODUCE_PRODUCT), status.Id, produce.Id, userId, produce.ProcedureNumber);
        return produce;
    }

    public async Task Update(OrderProduceProductModel form, int userId)
    {
        var produce = await _context.OrderProduceProducts.FindAsync(form.Id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }
        produce.CustomerId = form.CustomerId;
        produce.UserId = userId;
        //produce.Date = form.Date;
        produce.Note = form.Note;
        produce.UserUpdated = userId;
        produce.UpdatedAt = DateTime.Now;
        produce.IsSpecialOrder = form.IsSpecialOrder;

        _context.OrderProduceProducts.Update(produce);

        var detailDels = form.Items.Where(x => x.IsDeleted).ToList();
        if (detailDels.Any())
        {
            var detailDelIds = detailDels.Select(x => x.Id);
            var orderDetailDels = await _context.OrderProduceProductDetails.Where(x => detailDelIds.Contains(x.Id)).ToListAsync();

            // check detail IsProduced not remove
            if (orderDetailDels.Any(x => x.IsProduced))
            {
                throw new ErrorException(ErrorMessages.AccessDenined);
            }
            _context.OrderProduceProductDetails.RemoveRange(orderDetailDels);
        }
        var details = form.Items.Where(x => !x.IsDeleted).Select(x => new OrderProduceProductDetail
        {
            Id = x.Id,
            OrderProduceProductId = produce.Id,
            GoodsId = x.GoodsId,
            QuantityReal = x.QuantityReal,
            QuantityRequired = x.QuantityRequired,
            IsProduced = x.IsProduced,
            UnitPrice = x.UnitPrice,
            TaxVAT = x.TaxVAT,
            DiscountPrice = x.DiscountPrice,
        });
        _context.OrderProduceProductDetails.UpdateRange(details);

        // add promotions
        await _billPromotionService.Create(form.BillPromotions, form.Id, nameof(OrderProduceProduct));

        await _context.SaveChangesAsync();
    }

    public async Task Accept(int id, int userId, int year)
    {
        var produce = await _context.OrderProduceProducts.FindAsync(id);

        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }
        // validate condition
        var status = await _procedureOrderProduceProductHelperService.GetStatusAcceptForOrderProduceProduct(produce.ProcedureStatusId ?? 0, id, userId, year);
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        if (status.IsFinish)
        {
            produce.Date = DateTime.Now;
            produce.IsFinished = status.IsFinish;
            produce.Code = await GetCodeAsync();
        }

        produce.UpdatedAt = DateTime.Now;

        _context.OrderProduceProducts.Update(produce);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.ORDER_PRODUCE_PRODUCT), status.Id, userId, id, produce.ProcedureNumber);

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.ORDER_PRODUCE_PRODUCT), status.Id, id, userId, produce.ProcedureNumber, status.IsFinish);
    }

    public async Task Delete(int id)
    {
        var item = await _context.OrderProduceProducts.FindAsync(id);
        if (item.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }
        var isExistLog = await _procedureHelperService.ExistLog(id, nameof(ProcedureEnum.ORDER_PRODUCE_PRODUCT));

        if (isExistLog)
        {
            throw new ErrorException(ErrorMessages.ProcedureCannotDelete);
        }
        if (item is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        var details = await _context.OrderProduceProductDetails.Where(x => x.OrderProduceProductId == id).ToListAsync();
        if (details.Any(x => x.IsProduced))
        {
            throw new ErrorException(ErrorMessages.AccessDenined);
        }

        _context.OrderProduceProductDetails.RemoveRange(details);
        _context.OrderProduceProducts.Remove(item);

        await _procedureHelperService.DeleteLog(id, nameof(ProcedureEnum.ORDER_PRODUCE_PRODUCT));

        await _context.SaveChangesAsync();
    }

    public async Task<string> GetProcedureNumber()
    {
        var item = await _context.OrderProduceProducts.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        var procedureNumber = _procedureHelperService.GetProcedureNumber(item?.ProcedureNumber);
        return $"DHM-{procedureNumber}";
    }

    public async Task<OrderProduceProduct> CreateFromBill(int billId, int userId)
    {
        var billDetails = await _context.BillDetails.Where(x => x.BillId == billId).ToListAsync();
        var bill = await _context.Bills.FindAsync(billId);
        var data = new OrderProduceProductCreateModel
        {
            CustomerId = bill.CustomerId,
            Date = DateTime.Now,
            Items = billDetails.Select(x => new OrderProduceProductDetailSetterModel
            {
                GoodsId = x.GoodsId,
                DiscountPrice = x.DiscountPrice,
                UnitPrice = x.UnitPrice,
                TaxVAT = x.TaxVAT,
                Quantity = x.Quantity,
            }).ToList()
        };
        return await Create(data, userId);
    }

    public async Task NotAccept(int id, int userId)
    {
        var produce = await _context.OrderProduceProducts.FindAsync(id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }

        produce.NoteNotAccept = produce.ProcedureStatusName + "; " + produce.NoteNotAccept;
        // validate condition
        var status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.ORDER_PRODUCE_PRODUCT));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        produce.UpdatedAt = DateTime.Now;
        _context.OrderProduceProducts.Update(produce);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.ORDER_PRODUCE_PRODUCT), status.Id, userId, id, produce.ProcedureNumber, true);
    }

    private async Task<string> GetCodeAsync()
    {
        var month = DateTime.Today.Month;

        var codeNumber = await _context.OrderProduceProducts.Where(x => x.IsFinished && x.Date.Month == month)
            .OrderByDescending(x => x.Code).ThenByDescending(x => x.Id)
            .Select(x => x.Code).FirstOrDefaultAsync();
        return _procedureHelperService.GetCode(codeNumber, "Order");
    }

    public async Task SetIsProduced(IEnumerable<int> detailIds)
    {
        var details = await _context.OrderProduceProductDetails.Where(x => detailIds.Contains(x.Id)).ToListAsync();
        var relationships = await _context.PlanningProduceProductDetails.Where(x => detailIds.Contains(x.OrderProduceProductDetailId)).ToListAsync();

        foreach (var detail in details)
        {
            var quantity = relationships.Where(x => x.OrderProduceProductDetailId == detail.Id && !x.IsCanceled && string.IsNullOrEmpty(x.FileDeliveredStr)).Sum(x => x.Quantity);
            detail.IsProduced = detail.QuantityRequired <= quantity;
            detail.QuantityInProgress = quantity;
            detail.QuantityReal = detail.QuantityRequired - detail.QuantityInProgress - detail.QuantityDelivered;
        }

        _context.UpdateRange(details);
        await _context.SaveChangesAsync();
    }

    public async Task Canceled(int id)
    {
        var item = await _context.OrderProduceProducts.FindAsync(id);
        if (!item.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureNotFinished);
        }

        // check planning
        var existPlanningDetail = await _context.PlanningProduceProductDetails.AnyAsync(x => x.OrderProduceProductId == id);
        if (existPlanningDetail)
        {
            throw new ErrorException("Đã tồn tại kế hoạch sản xuất không thể xóa");
        }

        item.IsCanceled = true;
        _context.OrderProduceProducts.Update(item);

        await _context.SaveChangesAsync();
    }

}