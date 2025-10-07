using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.ProduceProductModels;
using ManageEmployee.DataTransferObject.SupplyModels;
using ManageEmployee.Entities.CarEntities;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.ProduceProductEntities;
using ManageEmployee.Extends;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Services.Interfaces.P_Procedures.Supplies;
using ManageEmployee.Services.Interfaces.ProduceProducts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.ProduceProductServices;
public class WarehouseProduceProductService: IWarehouseProduceProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;
    private readonly ICarDeliveryService _carDeliveryService;
    private readonly IPaymentProposalService _paymentProposalService;
    public WarehouseProduceProductService(ApplicationDbContext context, IMapper mapper, IProcedureHelperService procedureHelperService, ICarDeliveryService carDeliveryService, IPaymentProposalService paymentProposalService)
    {
        _context = context;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
        _carDeliveryService = carDeliveryService;
        _paymentProposalService = paymentProposalService;
    }


    public async Task<PagingResult<WarehouseProduceProductPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId)
    {
        var query = _context.WarehouseProduceProducts.Where(X => X.Id > 0);

        var startStatus = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.PLANNING_PRODUCE_WAREHOUSE));
        var statusIds = await _procedureHelperService.GetProcedureStatusIds(userId, param.StatusTab, nameof(ProcedureEnum.PLANNING_PRODUCE_WAREHOUSE));
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
            .Where(x => x.log.ProcedureCode == nameof(ProcedureEnum.PLANNING_PRODUCE_WAREHOUSE) && x.log.UserId == userId
                    && x.log.NotAcceptCount == 0)
            .Select(x => x.procedure).Distinct();
        }

        switch (param.StatusTab)
        {

            case ProduceProductStatusTab.Done:
                query = query.Where(x => x.IsFinished && !x.IsManufactureOrder);
                break;
            case ProduceProductStatusTab.Approved:
                query = query.Where(x => !x.IsFinished);
                break;
            case ProduceProductStatusTab.Finish:
                query = query.Where(x => x.IsManufactureOrder);
                break;
        }

        query = query.QueryDate(param);
        query = query.QuerySearchTextProcedure(param);

        var data = await query.OrderByDescending(x => x.CreatedAt).Skip(param.PageSize * param.Page).Take(param.PageSize)
            .Select(x => _mapper.Map<WarehouseProduceProductPagingModel>(x)).ToListAsync();
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

        return new PagingResult<WarehouseProduceProductPagingModel>
        {
            Data = data,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task<WarehouseProduceProductGetDetailModel> GetDetail(int id, int year)
    {
        var item = await _context.WarehouseProduceProducts.Where(x => x.Id == id).Select(x => _mapper.Map<WarehouseProduceProductGetDetailModel>(x)).FirstOrDefaultAsync();
        var details = await _context.WarehouseProduceProductDetails.Where(x => x.WarehouseProduceProductId == id).ToListAsync();
        var goodIds = details.Select(x => x.GoodsId).ToList();
        var goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();
        var carIds = details.Select(x => x.CarId).ToList();
        var detailIds = details.Select(x => x.Id);

        var carCodes = details.Select(x => x.CarName).Distinct().ToList();
        var carDeliveries = await _context.CarDeliveries.Where(X => X.TableName == nameof(WarehouseProduceProductDetail) && detailIds.Contains(X.TableId)).ToListAsync();

        var cars = carCodes.Select(x => new Car
        {
            Id = 0,
            LicensePlates = x
        });

        item.Items = new List<WarehouseProduceProductCarGetDetailModel>();
        var listStorege = await _context.GetChartOfAccount(year).Where(x => (x.Classification == 2 || x.Classification == 3) && !x.HasChild).ToListAsync();

        foreach (var car in cars)
        {
            var carPlanning = new WarehouseProduceProductCarGetDetailModel
            {
                CarId = details.FirstOrDefault(x => x.CarName == car.LicensePlates)?.CarId,
                CarName = car.LicensePlates,
                Goods = new List<WarehouseProduceProductGoodGetDetailModel>()
            };
            var goodDetails = details.Where(x => x.CarId == carPlanning.CarId && x.CarName == carPlanning.CarName).ToList();
            foreach (var itemDetail in goodDetails)
            {
                var goodDetail = new WarehouseProduceProductGoodGetDetailModel
                {
                    Id = itemDetail.Id,
                    CustomerId = itemDetail.CustomerId,
                    QuantityReal = itemDetail.QuantityReal,
                    QuantityRequired = itemDetail.QuantityRequired,
                    GoodsId = itemDetail.GoodsId,
                };
                goodDetail.CustomerName = await _context.Customers.Where(x => x.Id == itemDetail.CustomerId).Select(x => x.Name).FirstOrDefaultAsync();

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
                }

                carPlanning.Goods.Add(goodDetail);
            }
            item.Items.Add(carPlanning);
        }
        return item;
    }

    public async Task Create(int planningId, int userId)
    {
        var planning = await _context.PlanningProduceProducts.FindAsync(planningId);
        var planningDetails = await _context.PlanningProduceProductDetails.Where(x => x.PlanningProduceProductId == planningId).ToListAsync();

        var produce = new WarehouseProduceProduct
        {
            UserId = userId,
            Date = DateTime.Now,
            Note = planning.Note,
            CreatedAt = DateTime.Now,
            UserCreated = userId,
            UserUpdated = userId,
            UpdatedAt = DateTime.Now,
        };
        if (string.IsNullOrEmpty(produce.ProcedureNumber))
        {
            produce.ProcedureNumber = await GetProcedureNumber();
        }

        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.PLANNING_PRODUCE_WAREHOUSE));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;


        await _context.WarehouseProduceProducts.AddAsync(produce);
        await _context.SaveChangesAsync();

        var planningDetailIds = planningDetails.Select(x => x.Id).ToList();
        var carDeliveries = await _carDeliveryService.GetListForTableName(nameof(PlanningProduceProductDetail), planningDetailIds);
        var payments = await _paymentProposalService.GetListForTableName(nameof(PlanningProduceProductDetail), planningDetailIds);

        foreach (var planningDetail in planningDetails)
        {
            var detail = new WarehouseProduceProductDetail
            {
                WarehouseProduceProductId = produce.Id,
                GoodsId = planningDetail.GoodsId,
                QuantityReal = planningDetail.Quantity,
                QuantityRequired = planningDetail.Quantity,
                CustomerId = planningDetail.CustomerId,
                CarId = planningDetail.CarId,
                CarName = planningDetail.CarName,
            };
            await _context.WarehouseProduceProductDetails.AddAsync(detail);
            await _context.SaveChangesAsync();

            var carDelivery = carDeliveries.FirstOrDefault(x => x.TableId == planningDetail.Id);
            if (carDelivery != null)
            {
                await _carDeliveryService.Add(carDelivery, nameof(WarehouseProduceProductDetail), detail.Id);
            }

            var payment = payments.FirstOrDefault(x => x.TableId == planningDetail.Id);
            if (payment != null)
            {
                await _paymentProposalService.SetForOtherTable(payment, userId, detail.Id, nameof(WarehouseProduceProductDetail));
            }
        }
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.PLANNING_PRODUCE_WAREHOUSE), status.Id, produce.Id, userId, produce.ProcedureNumber);
    }


    public async Task Update(WarehouseProduceProductGetDetailModel form, int userId)
    {
        var produce = await _context.WarehouseProduceProducts.FindAsync(form.Id);

        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }
        produce.UserId = userId;
        produce.Note = form.Note;
        produce.UserUpdated = userId;
        produce.UpdatedAt = DateTime.Now;
        _context.WarehouseProduceProducts.Update(produce);

        var details = await _context.WarehouseProduceProductDetails.Where(x => x.WarehouseProduceProductId == form.Id).ToListAsync();
        var detailIds = form.Items.SelectMany(x => x.Goods).Select(x => x.Id);
        var detailDels = details.Where(x => !detailIds.Contains(x.Id)).ToList();
        // delete cardelivery
        if (detailDels.Any())
        {
            // check car
            var carDeliveries = await _carDeliveryService.GetListForTableName(nameof(WarehouseProduceProductDetail), detailDels.Select(x => x.Id));
            if (carDeliveries.Any())
            {
                foreach (var carDelivery in carDeliveries)
                {
                    var detailDel = details.FirstOrDefault(x => x.Id == carDelivery.TableId);
                    var detailOther = details.FirstOrDefault(x => x.CarId == detailDel.CarId && x.CarName == detailDel.CarName);
                    await _carDeliveryService.ResetTableId(carDelivery, detailOther.Id);
                }
            }
            // check payment
            var payments = await _paymentProposalService.GetListForTableName(nameof(WarehouseProduceProductDetail), detailDels.Select(x => x.Id));
            if (payments.Any())
            {
                foreach (var payment in payments)
                {
                    var detailDel = details.FirstOrDefault(x => x.Id == payment.TableId);
                    var detailOther = details.FirstOrDefault(x => x.CarId == detailDel.CarId && x.CarName == detailDel.CarName);
                    await _paymentProposalService.ResetTableId(payment, detailOther.Id);
                }
            }

            _context.WarehouseProduceProductDetails.RemoveRange(detailDels);
        }

        var detailUpdates = new List<WarehouseProduceProductDetail>();
        foreach (var item in form.Items)
        {
            if (item.Goods == null)
                continue;

            foreach (var itemGood in item.Goods)
            {
                var detail = details.FirstOrDefault(x => x.Id == itemGood.Id);
                if (detail is null)
                {
                    detail = new WarehouseProduceProductDetail();
                }

                detail.GoodsId = itemGood.GoodsId;
                detail.WarehouseProduceProductId = produce.Id;
                detail.QuantityReal = itemGood.QuantityReal;
                detail.QuantityRequired = itemGood.QuantityRequired;
                detail.CustomerId = itemGood.CustomerId;
                detail.CarId = item.CarId;
                detail.CarName = item.CarName;
                details.Add(detail);
            }
        }

        _context.WarehouseProduceProductDetails.UpdateRange(details);
        await _context.SaveChangesAsync();
    }

    public async Task Accept(int id, int userId)
    {
        var produce = await _context.WarehouseProduceProducts.FindAsync(id);
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

        _context.WarehouseProduceProducts.Update(produce);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.PLANNING_PRODUCE_WAREHOUSE), status.Id, userId, id, produce.ProcedureNumber);

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.PLANNING_PRODUCE_WAREHOUSE), status.Id, id, userId, produce.ProcedureNumber, status.IsFinish);
    }

    public async Task<string> GetProcedureNumber()
    {
        var item = await _context.WarehouseProduceProducts.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        var procedureNumber = _procedureHelperService.GetProcedureNumber(item?.ProcedureNumber);

        return $"{nameof(ProcedureEnum.PLANNING_PRODUCE_WAREHOUSE)}-{procedureNumber}";
        
    }
    public async Task NotAccept(int id, int userId)
    {
        var produce = await _context.WarehouseProduceProducts.FindAsync(id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }

        produce.NoteNotAccept = produce.ProcedureStatusName + "; " + produce.NoteNotAccept;
        // validate condition
        var status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.PLANNING_PRODUCE_WAREHOUSE));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        produce.UpdatedAt = DateTime.Now;
        _context.WarehouseProduceProducts.Update(produce);

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.PLANNING_PRODUCE_WAREHOUSE), status.Id, userId, id, produce.ProcedureNumber, true);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var item = await _context.WarehouseProduceProducts.FindAsync(id);
        if (item is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        if (item.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }
        var isExistLog = await _procedureHelperService.ExistLog(id, nameof(ProcedureEnum.PLANNING_PRODUCE_WAREHOUSE));
        if (isExistLog)
        {
            throw new ErrorException(ErrorMessages.ProcedureCannotDelete);
        }
        
        var details = await _context.WarehouseProduceProductDetails.Where(x => x.WarehouseProduceProductId == id).ToListAsync();

        _context.WarehouseProduceProductDetails.RemoveRange(details);
        _context.WarehouseProduceProducts.Remove(item);
        await _procedureHelperService.DeleteLog(id, nameof(ProcedureEnum.PLANNING_PRODUCE_WAREHOUSE));

        await _context.SaveChangesAsync();
    }

    public async Task<CarDeliveryModel> GetCarDelivery(int? carId, string carName, int id)
    {
        var planningDetails = await _context.WarehouseProduceProductDetails
                           .Where(X => X.WarehouseProduceProductId == id && X.CarId == carId && X.CarName == carName)
                           .ToListAsync();
        var planningDetailIds = planningDetails.Select(x => x.Id);
        var carDeliveryId = await _context.CarDeliveries.Where(X => X.TableName == nameof(WarehouseProduceProductDetail) && planningDetailIds.Contains(X.TableId)).FirstOrDefaultAsync();


        var data = (await _carDeliveryService.Get(carDeliveryId?.Id)) ?? new CarDeliveryModel();
        data.CarId = carId;
        data.CarName = carName;

        return data;
    }
    public async Task<PaymentProposalModel> GetPaymentProposal(int? carId, string carName, int id)
    {
        var planningDetailIds = await _context.WarehouseProduceProductDetails
                            .Where(X => X.WarehouseProduceProductId == id && X.CarId == carId && X.CarName == carName)
                            .Select(x => x.Id)
                            .ToListAsync();
        var paymentProposalId = await _context.PaymentProposals.Where(X => X.TableName == nameof(WarehouseProduceProductDetail) && planningDetailIds.Contains(X.TableId ?? 0)).FirstOrDefaultAsync();
        var data = await _paymentProposalService.GetDetail(paymentProposalId?.Id ?? 0) ?? new PaymentProposalModel();
        return data;
    }

    public async Task SetCarDelivery(CarDeliveryModel carDelivery, int id)
    {
        var planningDetailId = await _context.WarehouseProduceProductDetails
                            .Where(X => X.WarehouseProduceProductId == id && X.CarId == carDelivery.CarId && X.CarName == carDelivery.CarName)
                            .Select(x => x.Id)
                            .FirstOrDefaultAsync();

        await _carDeliveryService.Set(carDelivery, nameof(WarehouseProduceProductDetail), planningDetailId);
    }

    public async Task SetPaymentProposal(PaymentProposalModel model, int? carId, string carName, int id, int userId)
    {
        var planningDetailId = await _context.WarehouseProduceProductDetails
                            .Where(X => X.WarehouseProduceProductId == id && X.CarId == carId && X.CarName == carName)
                            .Select(x => x.Id)
                            .FirstOrDefaultAsync();
        await _paymentProposalService.Create(model, userId, planningDetailId, nameof(WarehouseProduceProductDetail));
    }
    private async Task<string> GetCodeAsync()
    {
        var month = DateTime.Today.Month;
        var codeNumber = await _context.WarehouseProduceProducts.Where(x => x.IsFinished && x.Date.Month == month).OrderByDescending(x => x.Id).Select(x => x.Code).FirstOrDefaultAsync();
        return _procedureHelperService.GetCode(codeNumber, "Warehouse");
    }
}
