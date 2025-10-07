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
using ManageEmployee.Services.Interfaces.Ledgers;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Services.Interfaces.ProduceProducts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.ProduceProductServices;

public class ProduceProductService : IProduceProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;
    private readonly ILedgerForSaleService _ledgerForSaleService;
    public ProduceProductService(ApplicationDbContext context, IMapper mapper, IProcedureHelperService procedureHelperService, ILedgerForSaleService ledgerForSaleService)
    {
        _context = context;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
        _ledgerForSaleService = ledgerForSaleService;
    }

    public async Task<PagingResult<ProduceProductPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId)
    {
        var query = _context.ProduceProducts.Where(X => X.Id > 0);

        var startStatus = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.PRODUCE_PRODUCT));
        var statusIds = await _procedureHelperService.GetProcedureStatusIds(userId, param.StatusTab, nameof(ProcedureEnum.PRODUCE_PRODUCT));
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
            .Where(x => x.log.ProcedureCode == nameof(ProcedureEnum.PRODUCE_PRODUCT) && x.log.UserId == userId
                        && x.log.NotAcceptCount == 0)
            .Select(x => x.procedure).Distinct();
        }
        query = query.QueryDate(param);
        query = query.QuerySearchTextProcedure(param);

        var data = await query.OrderByDescending(x => x.CreatedAt).Skip(param.PageSize * param.Page).Take(param.PageSize)
            .Select(x => _mapper.Map<ProduceProductPagingModel>(x)).ToListAsync();

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

        return new PagingResult<ProduceProductPagingModel>
        {
            Data = data,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task<ProduceProductModel> GetDetail(int id, int year)
    {
        var item = await _context.ProduceProducts.Where(x => x.Id == id).Select(x => _mapper.Map<ProduceProductModel>(x)).FirstOrDefaultAsync();
        var details = await _context.ProduceProductDetails.Where(x => x.ProduceProductId == id).ToListAsync();
        var detailIds = details.Select(x => x.Id);
        var goodIds = details.Select(x => x.GoodsId).ToList();
        var goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();

        item.Items = new List<ProduceProductDetailModel>();
        var listStorege = await _context.GetChartOfAccount(year).Where(x => (x.Classification == 2 || x.Classification == 3) && !x.HasChild).ToListAsync();

        foreach (var itemDetail in details)
        {
            var goodDetail = new ProduceProductDetailModel
            {
                Id = itemDetail.Id,
                CustomerId = itemDetail.CustomerId,
                QuantityReal = itemDetail.QuantityReal,
                QuantityRequired = itemDetail.QuantityRequired,
                GoodsId = itemDetail.GoodsId,
            };
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
                item.Items.Add(goodDetail);
            }
        }

        return item;
    }

    public async Task Create(int manufactureOrderId, int userId)
    {
        var manufactureOrder = await _context.ManufactureOrders.FindAsync(manufactureOrderId);

        var produce = new ProduceProduct
        {
            UserId = userId,
            Date = DateTime.Now,
            Note = manufactureOrder.Note,
            CreatedAt = DateTime.Now,
            UserCreated = userId,
            UserUpdated = userId,
            UpdatedAt = DateTime.Now,
            ProcedureNumber = await GetProcedureNumber(),
            ManufactureOrderCode = manufactureOrder.ProcedureNumber,
            ManufactureOrderId = manufactureOrderId,
            ResponsibleUserId = manufactureOrder.ResponsibleUserId
        };

        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.PRODUCE_PRODUCT));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;
        await _context.ProduceProducts.AddAsync(produce);
        await _context.SaveChangesAsync();

        var details = await _context.ManufactureOrderDetails.Where(x => x.ManufactureOrderId == manufactureOrderId).ToListAsync();
        var detailAdds = details.ConvertAll(x => new ProduceProductDetail
        {
            CarId = x.CarId,
            CarName = x.CarName,
            CustomerId = x.CustomerId,
            GoodsId = x.GoodsId,
            ProduceProductId = produce.Id,
            QuantityReal = x.QuantityReal,
            QuantityRequired = x.QuantityReal,
        }).ToList();
        await _context.ProduceProductDetails.AddRangeAsync(detailAdds);
        await _context.SaveChangesAsync();


        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.PRODUCE_PRODUCT), status.Id, produce.Id, userId, produce.ProcedureNumber);
    }

    public async Task Update(ProduceProductModel form, int userId)
    {
        var produce = await _context.ProduceProducts.FindAsync(form.Id);

        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }
        produce.UserId = userId;
        produce.Note = form.Note;
        produce.UserUpdated = userId;
        produce.UpdatedAt = DateTime.Now;
        _context.ProduceProducts.Update(produce);
        //
        var details = await _context.ProduceProductDetails.Where(x => x.ProduceProductId == form.Id).ToListAsync();
        var detailUpdateIds = form.Items.Select(X => X.Id).ToList();
        var detailDels = details.Where(x => !detailUpdateIds.Contains(x.Id)).ToList();
        _context.ProduceProductDetails.RemoveRange(detailDels);

        var detailUpdates = form.Items.ConvertAll(x => new ProduceProductDetail
        {
            Id = x.Id,
            CarId = x.CarId,
            CarName = x.CarName,
            CustomerId = x.CustomerId,
            GoodsId = x.GoodsId,
            ProduceProductId = produce.Id,
            QuantityReal = x.QuantityReal,
            QuantityRequired = x.QuantityRequired,
        }).ToList();

        _context.ProduceProductDetails.UpdateRange(detailUpdates);
        await _context.SaveChangesAsync();
    }

    public async Task Accept(int id, int userId)
    {
        var produce = await _context.ProduceProducts.FindAsync(id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }
        // validate condition
        var status = await _procedureHelperService.GetStatusAccept(produce.ProcedureStatusId ?? 0, userId);
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        if (status.IsFinish)
        {
            produce.IsFinished = status.IsFinish;
            produce.Code = await GetCodeAsync();
        }

        produce.UpdatedAt = DateTime.Now;

        _context.ProduceProducts.Update(produce);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.PRODUCE_PRODUCT), status.Id, userId, id, produce.ProcedureNumber);

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.PRODUCE_PRODUCT), status.Id, id, userId, produce.ProcedureNumber, status.IsFinish);
    }

    public async Task NotAccept(int id, int userId)
    {
        var produce = await _context.ProduceProducts.FindAsync(id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }

        produce.NoteNotAccept = produce.ProcedureStatusName + "; " + produce.NoteNotAccept;
        // validate condition
        var status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.PRODUCE_PRODUCT));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        produce.UpdatedAt = DateTime.Now;
        _context.ProduceProducts.Update(produce);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.PRODUCE_PRODUCT), status.Id, userId, id, produce.ProcedureNumber, true);
    }

    private async Task<string> GetProcedureNumber()
    {
        var item = await _context.ProduceProducts.OrderByDescending(x => x.ProcedureNumber).FirstOrDefaultAsync();
        var procedureNumber = _procedureHelperService.GetProcedureNumber(item?.ProcedureNumber);

        return $"{nameof(ProcedureEnum.PRODUCE_PRODUCT)}-{procedureNumber}";
    }

    public async Task Delete(int id)
    {
        var item = await _context.ProduceProducts.FindAsync(id);
        if (item is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        if (item.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }
        var isExistLog = await _procedureHelperService.ExistLog(id, nameof(ProcedureEnum.PRODUCE_PRODUCT));

        if (isExistLog)
        {
            throw new ErrorException(ErrorMessages.ProcedureCannotDelete);
        }
        
        var details = await _context.ProduceProductDetails.Where(x => x.ProduceProductId == id).ToListAsync();
        _context.ProduceProductDetails.RemoveRange(details);
        _context.ProduceProducts.Remove(item);

        await _procedureHelperService.DeleteLog(id, nameof(ProcedureEnum.PRODUCE_PRODUCT));

        await _context.SaveChangesAsync();
    }

    public async Task SetLedgerImportProduct(int id, int year)
    {
        var item = await _context.ProduceProducts.FindAsync(id);
        if (!item.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureNotFinished);
        }
        if (item.IsImportProduct)
        {
            throw new ErrorException("Quy trình đã nhập kho thành phẩm");
        }

        item.IsImportProduct = true;
        _context.ProduceProducts.Update(item);
        await _context.SaveChangesAsync();
        var details = await _context.ProduceProductDetails.Where(X => X.ProduceProductId == id && X.QuantityReal > 0).ToListAsync();
       await _ledgerForSaleService.AddLedgerProduceProduct(details, "NK", year, item.ResponsibleUserId);
    }

    public async Task SetLedgerExportProduct(int id, int year)
    {
        var item = await _context.ProduceProducts.FindAsync(id);
        if (!item.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureNotFinished);
        }

        if (item.IsExportProduct)
        {
            throw new ErrorException("Quy trình đã xuất kho nguyên liệu");
        }
        item.IsExportProduct = true;
        _context.ProduceProducts.Update(item);
        await _context.SaveChangesAsync();
        var details = await _context.ProduceProductDetails.Where(X => X.ProduceProductId == id && X.QuantityReal > 0).ToListAsync();
        await _ledgerForSaleService.AddLedgerProduceProduct(details, "XK", year, item.ResponsibleUserId);
    }

    public async Task<IEnumerable<ProduceProductGetListModel>> GetList()
    {
        return await _context.ProduceProducts.Where(x => x.IsFinished).Select(x => _mapper.Map<ProduceProductGetListModel>(x)).ToListAsync();
    }

    private async Task<string> GetCodeAsync()
    {
        var month = DateTime.Today.Month;

        var codeNumber = await _context.ProduceProducts.Where(x => x.IsFinished && x.Date.Month == month).OrderByDescending(x => x.Id).Select(x => x.Code).FirstOrDefaultAsync();
        return _procedureHelperService.GetCode(codeNumber, "Product");
    }
}