using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.ProduceProductModels;
using ManageEmployee.Entities.CarEntities;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.ProduceProductEntities;
using ManageEmployee.Extends;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Services.Interfaces.ProduceProducts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.ProduceProductServices.ManufactureOrderServices;

public class ManufactureOrderService : IManufactureOrderService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;
    private readonly ICarDeliveryService _carDeliveryService;
    private readonly IProduceProductService _produceProductService;
    public ManufactureOrderService(ApplicationDbContext context, IMapper mapper,
        IProcedureHelperService procedureHelperService,
        ICarDeliveryService carDeliveryService,
        IProduceProductService produceProductService)
    {
        _context = context;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
        _carDeliveryService = carDeliveryService;
        _produceProductService = produceProductService;
    }

    public async Task<PagingResult<ManufactureOrderPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId)
    {
        var query = _context.ManufactureOrders.Where(X => X.Id > 0);

        var startStatus = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.MANUFACTURE_ORDER));
        var statusIds = await _procedureHelperService.GetProcedureStatusIds(userId, param.StatusTab, nameof(ProcedureEnum.MANUFACTURE_ORDER));
        query = query.Where(x => statusIds.Contains(x.ProcedureStatusId));

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
            .Where(x => x.log.ProcedureCode == nameof(ProcedureEnum.MANUFACTURE_ORDER) && x.log.UserId == userId
                        && x.log.NotAcceptCount == 0)
            .Select(x => x.procedure).Distinct();
        }

        switch (param.StatusTab)
        {

            case ProduceProductStatusTab.Done:
                query = query.Where(x => x.IsFinished && !x.IsCanceled);
                break;
            case ProduceProductStatusTab.Approved:
                query = query.Where(x => !x.IsFinished);
                break;
            case ProduceProductStatusTab.Cancel:
                query = query.Where(x => x.IsCanceled);
                break;
        }
        query = query.QueryDate(param);
        query = query.QuerySearchTextProcedure(param);

        var data = await query.OrderByDescending(x => x.CreatedAt).Skip(param.PageSize * param.Page).Take(param.PageSize)
            .Select(x => _mapper.Map<ManufactureOrderPagingModel>(x)).ToListAsync();

        foreach (var item in data)
        {
            item.ShoulDelete = param.StatusTab == ProduceProductStatusTab.Pending && item.ProcedureStatusName == startStatus.P_StatusName;
            item.ShoulNotAccept = param.StatusTab == ProduceProductStatusTab.Pending && item.ProcedureStatusName != startStatus.P_StatusName;
            if (item.IsFinished)
            {
                item.ProcedureNumber = item.Code;
            }
        }
        var totalItem = await query.CountAsync();

        return new PagingResult<ManufactureOrderPagingModel>
        {
            Data = data,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task<ManufactureOrderGetDetailModel> GetDetail(int id, int year)
    {
        var item = await _context.ManufactureOrders.Where(x => x.Id == id).Select(x => _mapper.Map<ManufactureOrderGetDetailModel>(x)).FirstOrDefaultAsync();
        var details = await _context.ManufactureOrderDetails.Where(x => x.ManufactureOrderId == id).ToListAsync();
        var detailIds = details.Select(x => x.Id);
        var goodIds = details.Select(x => x.GoodsId).ToList();
        var goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();
        var carCodes = details.Select(x => x.CarName).Distinct().ToList();
        var carDeliveries = await _context.CarDeliveries.Where(X => X.TableName == nameof(ManufactureOrderDetail) && detailIds.Contains(X.TableId)).ToListAsync();

        var cars = carCodes.Select(x => new Car
        {
            Id = 0,
            LicensePlates = x
        });
        var detailAdds = new List<ManufactureOrderGoodGetDetailModel>();
        var listStorege = await _context.GetChartOfAccount(year).Where(x => (x.Classification == 2 || x.Classification == 3) && !x.HasChild).ToListAsync();

        var customerIds = details.Select(x => x.CustomerId).ToList();
        var customers = await _context.Customers.Where(x => customerIds.Contains(x.Id)).ToListAsync();

        foreach (var itemDetail in details)
        {
            var goodDetail = new ManufactureOrderGoodGetDetailModel
            {
                Id = itemDetail.Id,
                CustomerId = itemDetail.CustomerId,
                QuantityReal = itemDetail.QuantityReal,
                QuantityRequired = itemDetail.QuantityRequired,
                GoodsId = itemDetail.GoodsId,
                Note = itemDetail.Note,
            };
            goodDetail.CustomerName = customers.FirstOrDefault(x => x.Id == itemDetail.CustomerId)?.Name;

            var good = goods.FirstOrDefault(x => x.Id == itemDetail.GoodsId);
            if (good != null)
            {
                goodDetail.GoodsCode = GoodNameGetter.GetCodeFromGood(good);
                goodDetail.GoodsName = GoodNameGetter.GetNameFromGood(good);
                goodDetail.StockUnit = good.StockUnit;

                ChartOfAccount storegeDetail;
                if (!string.IsNullOrEmpty(good.Detail2))
                {
                    string parentRef = good.Account + ":" + good.Detail1;
                    storegeDetail = listStorege.Find(y => y.Code == good.Detail2 && y.ParentRef == parentRef &&
                            (string.IsNullOrEmpty(good.Warehouse) || y.WarehouseCode == good.Warehouse));
                }
                else if (!string.IsNullOrEmpty(good.Detail1))
                    storegeDetail = listStorege.Find(y => y.Code == good.Detail1 && y.ParentRef == good.Account &&
                    (string.IsNullOrEmpty(good.Warehouse) || y.WarehouseCode == good.Warehouse));
                else
                    storegeDetail = listStorege.Find(y => y.Code == good.Account);

                if (storegeDetail != null)
                {
                    goodDetail.QuantityStock = (storegeDetail.OpeningStockQuantityNB ?? 0) + (storegeDetail.ArisingStockQuantityNB ?? 0);
                }

                if (good.GoodsType == nameof(GoodsTypeEnum.DM))
                {
                    var goodQuotes = await _context.GoodDetails.Where(x => x.GoodID == itemDetail.GoodsId).ToListAsync();

                    goodDetail.GoodDetails = new List<WarehouseProduceProductGoodDetailModel>();
                    foreach (var goodQuote in goodQuotes)
                    {
                        var goodQuoteDetail = new WarehouseProduceProductGoodDetailModel
                        {
                            GoodsCode = GoodNameGetter.GetCodeFromGoodDetail(goodQuote),
                            GoodsName = GoodNameGetter.GetNameFromGoodDetail(goodQuote),
                            QuantityRequired = (goodQuote.Quantity ?? 0) * goodDetail.QuantityRequired,
                            QuantityReal = (goodQuote.Quantity ?? 0) * goodDetail.QuantityReal,
                        };

                        ChartOfAccount storegeQuoteDetail;
                        if (!string.IsNullOrEmpty(good.Detail2))
                        {
                            string parentRef = good.Account + ":" + good.Detail1;
                            storegeQuoteDetail = listStorege.Find(y => y.Code == good.Detail2 && y.ParentRef == parentRef &&
                                    (string.IsNullOrEmpty(good.Warehouse) || y.WarehouseCode == good.Warehouse));
                        }
                        else if (!string.IsNullOrEmpty(good.Detail1))
                            storegeQuoteDetail = listStorege.Find(y => y.Code == good.Detail1 && y.ParentRef == good.Account &&
                            (string.IsNullOrEmpty(good.Warehouse) || y.WarehouseCode == good.Warehouse));
                        else
                            storegeQuoteDetail = listStorege.Find(y => y.Code == good.Account);
                        if (storegeQuoteDetail != null)
                        {
                            goodQuoteDetail.QuantityStock = (storegeQuoteDetail.OpeningStockQuantityNB ?? 0) + (storegeQuoteDetail.ArisingStockQuantityNB ?? 0);
                            goodQuoteDetail.StockUnit = storegeQuoteDetail.StockUnit;
                        }
                        goodDetail.GoodDetails.Add(goodQuoteDetail);
                    }
                }
                detailAdds.Add(goodDetail);
            }
        }

        var detailTotal = new ManufactureOrderGoodGetDetailModel
        {
            QuantityReal = detailAdds.Sum(x => x.QuantityReal),
            QuantityRequired = detailAdds.Sum(x => x.QuantityRequired),
            QuantityStock = detailAdds.Sum(x => x.QuantityStock),
            GoodsName = "Tổng cộng"
        };
        item.Items = new List<ManufactureOrderGoodGetDetailModel>
        {
            detailTotal,
        };
        item.Items.AddRange(detailAdds);

        return item;
    }

    public async Task<IEnumerable<ManufactureOrderPagingModel>> GetList()
    {
        return await _context.ManufactureOrders.Where(x => !x.IsFinished).Select(x => _mapper.Map<ManufactureOrderPagingModel>(x)).ToListAsync();
    }

    public async Task Create(ManufactureOrderModel form, int userId)
    {
        var produce = new ManufactureOrder
        {
            UserId = userId,
            Date = DateTime.Now,
            Note = form.Note,
            CreatedAt = DateTime.Now,
            UserCreated = userId,
            UserUpdated = userId,
            UpdatedAt = DateTime.Now,
        };
        if (string.IsNullOrEmpty(produce.ProcedureNumber))
        {
            produce.ProcedureNumber = await GetProcedureNumber();
        }

        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.MANUFACTURE_ORDER));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;
        await _context.ManufactureOrders.AddAsync(produce);
        await _context.SaveChangesAsync();

        await AddDetail(form.Items, produce.Id);

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.MANUFACTURE_ORDER), status.Id, produce.Id, userId, produce.ProcedureNumber);
    }

    public async Task Update(ManufactureOrderModel form, int userId)
    {
        var produce = await _context.ManufactureOrders.FindAsync(form.Id);

        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }
        produce.UserId = userId;
        produce.Note = form.Note;
        produce.UserUpdated = userId;
        produce.UpdatedAt = DateTime.Now;
        _context.ManufactureOrders.Update(produce);
        await _context.SaveChangesAsync();

        await AddDetail(form.Items, produce.Id);
    }

    private async Task AddDetail(List<ManufactureOrderDetailModel> Items, int produceId)
    {
        var planningDetailIds = Items.Select(x => x.Id).ToList();
        var itemChecks = await _context.ManufactureOrderDetails.Where(x => x.ManufactureOrderId == produceId).ToListAsync();
        var warehousePlanningProduceProductDetailIds = Items.Select(x => x.WarehousePlanningProduceProductDetailId);
        var warehousePlanningProduceProductIds = await _context.WarehouseProduceProductDetails.Where(x => warehousePlanningProduceProductDetailIds.Contains(x.Id)).Select(x => x.WarehouseProduceProductId).Distinct().ToListAsync();
        var warehousePlanningProduceProducts = await _context.WarehouseProduceProducts.Where(x => warehousePlanningProduceProductIds.Contains(x.Id) && !x.IsManufactureOrder).ToListAsync();
        warehousePlanningProduceProducts = warehousePlanningProduceProducts.ConvertAll(x =>
        {
            x.IsManufactureOrder = true;
            return x;
        });
        _context.WarehouseProduceProducts.UpdateRange(warehousePlanningProduceProducts);

        foreach (var item in Items)
        {
            var detail = itemChecks.FirstOrDefault(x => x.GoodsId == item.GoodsId && x.CustomerId == item.CustomerId);
            if (detail is null)
            {
                detail = new ManufactureOrderDetail
                {
                    ManufactureOrderId = produceId,
                    GoodsId = item.GoodsId,
                    CustomerId = item.CustomerId,
                    WarehousePlanningProduceProductDetailId = item.WarehousePlanningProduceProductDetailId,
                    Note = item.Note,
                    //CarId = item.CarId,
                    //CarName = item.CarName
                };
            }
            detail.QuantityReal += item.QuantityReal;
            detail.QuantityRequired += item.QuantityRequired;

            _context.ManufactureOrderDetails.Update(detail);
            await _context.SaveChangesAsync();
        }
        await _context.SaveChangesAsync();
    }

    public async Task UpdateForManufacture(ManufactureOrderGetDetailModel form, int userId)
    {
        var produce = await _context.ManufactureOrders.FindAsync(form.Id);

        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }
        produce.UserId = userId;
        produce.Note = form.Note;
        produce.UserUpdated = userId;
        produce.UpdatedAt = DateTime.Now;

        if (produce.IsCanceled && string.IsNullOrEmpty(form.CanceledNote))
        {
            throw new ErrorException(ErrorMessages.CannotAccept);
        }
        produce.CanceledNote = form.CanceledNote;
        produce.IsCanceled = !string.IsNullOrEmpty(form.CanceledNote);

        _context.ManufactureOrders.Update(produce);

        var detailIdUpdates = form.Items.Select(x => x.Id);
        var detailDels = await _context.ManufactureOrderDetails.Where(X => X.ManufactureOrderId == form.Id && !detailIdUpdates.Contains(X.Id)).ToListAsync();
        _context.ManufactureOrderDetails.RemoveRange(detailDels);

        var details = await _context.ManufactureOrderDetails.Where(X => X.ManufactureOrderId == form.Id && detailIdUpdates.Contains(X.Id)).ToListAsync();
        if (form.Items != null)
        {

            foreach (var detail in details)
            {
                var item = form.Items.FirstOrDefault(x => x.Id == detail.Id);
                if (item is null)
                    continue;
                detail.QuantityReal = item.QuantityReal;
                detail.CustomerId = item.CustomerId;
                detail.Note = item.Note;
            }
            _context.ManufactureOrderDetails.UpdateRange(details);

            var detailAdds = form.Items.Where(x => x.Id == 0 && x.GoodsId > 0)
                .Select(x => new ManufactureOrderDetail
                {
                    ManufactureOrderId = form.Id,
                    GoodsId = x.GoodsId,
                    CustomerId = x.CustomerId,
                    QuantityReal = x.QuantityReal,
                    QuantityRequired = x.QuantityRequired,
                    CreatedAt = DateTime.Now,
                    Note = x.Note
                });
            await _context.ManufactureOrderDetails.AddRangeAsync(detailAdds);
        }
        await _context.SaveChangesAsync();
    }

    public async Task Accept(int id, int userId)
    {
        var produce = await _context.ManufactureOrders.FindAsync(id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }
        // validate condition
        var status = await _procedureHelperService.GetStatusAccept(produce.ProcedureStatusId ?? 0, userId);
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        if (!produce.IsProduceProduct && (status.IsFinish || status.ProcedureConditionCode == nameof(ProcedureOrderProduceProductConditionEnum.ProduceProduct)))
        {
            if(produce.ResponsibleUserId is null)
            {
                throw new ErrorException("Bạn chưa chọn người chịu trách nhiệm");
            }
            produce.IsProduceProduct = true;
            await _produceProductService.Create(id, userId);
        }
        if (status.IsFinish)
        {
            produce.IsFinished = status.IsFinish;
            produce.Code = await GetCodeAsync();
        }
        produce.UpdatedAt = DateTime.Now;

        _context.ManufactureOrders.Update(produce);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.MANUFACTURE_ORDER), status.Id, userId, id, produce.ProcedureNumber);

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.MANUFACTURE_ORDER), status.Id, id, userId, produce.ProcedureNumber, status.IsFinish);
    }

    public async Task NotAccept(int id, int userId)
    {
        var produce = await _context.ManufactureOrders.FindAsync(id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }

        produce.NoteNotAccept = produce.ProcedureStatusName + "; " + produce.NoteNotAccept;
        // validate condition
        var status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.MANUFACTURE_ORDER));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        produce.UpdatedAt = DateTime.Now;
        _context.ManufactureOrders.Update(produce);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.MANUFACTURE_ORDER), status.Id, userId, id, produce.ProcedureNumber, true);
    }

    private async Task<string> GetProcedureNumber()
    {
        var item = await _context.ManufactureOrders.OrderByDescending(x => x.ProcedureNumber).FirstOrDefaultAsync();
        var procedureNumber = _procedureHelperService.GetProcedureNumber(item?.ProcedureNumber);

        return $"{nameof(ProcedureEnum.MANUFACTURE_ORDER)}-{procedureNumber}";
    }

    public async Task Delete(int id)
    {
        var isExistLog = await _procedureHelperService.ExistLog(id, nameof(ProcedureEnum.MANUFACTURE_ORDER));
        if (isExistLog)
        {
            throw new ErrorException(ErrorMessages.ProcedureCannotDelete);
        }

        var item = await _context.ManufactureOrders.FindAsync(id);
        if (item is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        if (item.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }

        var details = await _context.ManufactureOrderDetails.Where(x => x.ManufactureOrderId == id).ToListAsync();
        _context.ManufactureOrderDetails.RemoveRange(details);
        _context.ManufactureOrders.Remove(item);

        await _procedureHelperService.DeleteLog(id, nameof(ProcedureEnum.MANUFACTURE_ORDER));

        await _context.SaveChangesAsync();
    }
    private async Task<string> GetCodeAsync()
    {
        var month = DateTime.Today.Month;

        var codeNumber = await _context.ManufactureOrders.Where(x => x.IsFinished && x.Date.Month == month).OrderByDescending(x => x.Id).Select(x => x.Code).FirstOrDefaultAsync();
        return _procedureHelperService.GetCode(codeNumber, "Manufacture");
    }

    public async Task UpdateManufactureFromPaging(ManufactureOrderPagingModel form)
    {
        var item = await _context.ManufactureOrders.FindAsync(form.Id);
        if (item is null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        if (item.IsCanceled && string.IsNullOrEmpty(form.CanceledNote))
        {
            throw new ErrorException(ErrorMessages.CannotAccept);
        }

        item.Note = form.Note;
        item.CanceledNote = form.CanceledNote;
        item.IsCanceled = !string.IsNullOrEmpty(form.CanceledNote);
        _context.ManufactureOrders.Update(item);
        await _context.SaveChangesAsync();
    }

    public async Task SetResponsibleUser(int id, int responsibleUserId, int userId)
    {
        var produce = await _context.ManufactureOrders.FindAsync(id);
        //if (!produce.IsFinished)
        //{
        //    throw new ErrorException(ErrorMessages.ProcedureNotFinished);
        //}
        produce.ResponsibleUserId = responsibleUserId;
        produce.UserUpdated = userId;
        produce.UpdatedAt = DateTime.Now;
        _context.ManufactureOrders.Update(produce);
        await _context.SaveChangesAsync();
    }
}