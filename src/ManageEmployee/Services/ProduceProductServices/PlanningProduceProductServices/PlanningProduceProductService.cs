using AutoMapper;
using ManageEmployee.Extends;
using ManageEmployee.Helpers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.Bills;
using ManageEmployee.Services.Interfaces.P_Procedures.Supplies;
using ManageEmployee.Services.Interfaces.P_Procedures;
using ManageEmployee.Services.Interfaces.ProduceProducts.OrderProduceProducts;
using ManageEmployee.Services.Interfaces.ProduceProducts.PlanningProduceProducts;
using ManageEmployee.Services.Interfaces.ProduceProducts;
using ManageEmployee.DataTransferObject.CarModels;
using ManageEmployee.DataTransferObject.ProduceProductModels;
using ManageEmployee.DataTransferObject.SupplyModels;
using ManageEmployee.DataTransferObject.P_ProcedureView;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.ProduceProductEntities;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.DataTransferObject.Enums;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.FileModels;
using ManageEmployee.Entities.CarEntities;
using Common.Constants;

namespace ManageEmployee.Services.ProduceProductServices.PlanningProduceProductServices;

public class PlanningProduceProductService : IPlanningProduceProductService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IProcedureHelperService _procedureHelperService;
    private readonly IWarehouseProduceProductService _warehouseProduceProductService;
    private readonly ICarDeliveryService _carDeliveryService;
    private readonly IPaymentProposalService _paymentProposalService;
    private readonly IOrderProduceProductService _orderProduceProductService;
    private readonly IBillPromotionService _billPromotionService;
    private readonly IProduceProductLedgerService _produceProductLedgerService;
    private readonly IProcedurePlanningProduceProductHelper _procedurePlanningProduceProductHelper;
    private readonly ILogger<PlanningProduceProductService> _logger;
    public PlanningProduceProductService(ApplicationDbContext context,
        IMapper mapper,
        IProcedureHelperService procedureHelperService,
        IWarehouseProduceProductService warehouseProduceProductService,
        ICarDeliveryService carDeliveryService,
        IPaymentProposalService paymentProposalService,
        IOrderProduceProductService orderProduceProductService,
        IBillPromotionService billPromotionService,
        IProduceProductLedgerService produceProductLedgerService, 
        IProcedurePlanningProduceProductHelper procedurePlanningProduceProductHelper, 
        ILogger<PlanningProduceProductService> logger)
    {
        _context = context;
        _mapper = mapper;
        _procedureHelperService = procedureHelperService;
        _warehouseProduceProductService = warehouseProduceProductService;
        _carDeliveryService = carDeliveryService;
        _paymentProposalService = paymentProposalService;
        _orderProduceProductService = orderProduceProductService;
        _billPromotionService = billPromotionService;
        _produceProductLedgerService = produceProductLedgerService;
        _procedurePlanningProduceProductHelper = procedurePlanningProduceProductHelper;
        _logger = logger;
    }

    public async Task<PagingResult<PlanningProduceProductPagingModel>> GetPaging(ProcedurePagingRequestModel param, int userId)
    {
        var query = _context.PlanningProduceProducts.Where(x => x.Id > 0);
        var startStatus = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.PLANNING_PRODUCE_PRODUCT));
        var statusIds = await _procedureHelperService.GetProcedureStatusIds(userId, param.StatusTab, nameof(ProcedureEnum.PLANNING_PRODUCE_PRODUCT));
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
            .Where(x => x.log.ProcedureCode == nameof(ProcedureEnum.PLANNING_PRODUCE_PRODUCT) && x.log.UserId == userId)
            .Select(x => x.procedure).Distinct();
        }
        switch (param.StatusTab)
        {
            case ProduceProductStatusTab.Cancel:
                query = query.Where(x => x.IsCanceled);
                break;
            case ProduceProductStatusTab.Done:
                query = query.Where(x => x.IsFinished && !x.IsCanceled && !x.IsPart);
                break;
            case ProduceProductStatusTab.Finish:
                query = query.Where(x => x.IsDone);
                break;
            case ProduceProductStatusTab.Part:
                query = query.Where(x => x.IsPart);
                break;
            case ProduceProductStatusTab.Approved:
                query = query.Where(x => !x.IsFinished);
                break;
        }

        query = query.QueryDate(param);
        query = query.QuerySearchTextProcedure(param);

        var data = await query.OrderByDescending(x => x.CreatedAt).Skip(param.PageSize * param.Page).Take(param.PageSize)
            .Select(x => _mapper.Map<PlanningProduceProductPagingModel>(x)).ToListAsync();
        var statuses = await _context.Status.Where(x => x.Type == StatusTypeEnum.ProduceProduct).ToListAsync();

        foreach (var item in data)
        {
            item.Quantity = await _context.PlanningProduceProductDetails.Where(x => x.PlanningProduceProductId == item.Id).SumAsync(x => x.Quantity);
            item.ShoulDelete = param.StatusTab == ProduceProductStatusTab.Pending && item.ProcedureStatusName == startStatus.P_StatusName;
            item.ShoulNotAccept = param.StatusTab == ProduceProductStatusTab.Pending && item.ProcedureStatusName != startStatus.P_StatusName;
            if (item.IsFinished)
            {
                item.ProcedureNumber = item.Code;
            }
        }

        var totalItem = await query.CountAsync();

        return new PagingResult<PlanningProduceProductPagingModel>
        {
            Data = data,
            TotalItems = totalItem,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        };
    }

    public async Task<PlanningProduceProductGetDetailModel> GetDetail(int id, int userId)
    {
        var item = await _context.PlanningProduceProducts.FirstOrDefaultAsync(x => x.Id == id);
        var itemOut = _mapper.Map<PlanningProduceProductGetDetailModel>(item);
        var details = await _context.PlanningProduceProductDetails.Where(x => x.PlanningProduceProductId == id).ToListAsync();
        var detailIds = details.Select(x => x.Id);
        var goodIds = details.Select(x => x.GoodsId).ToList();
        var goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();
        var customerIds = details.Select(x => x.CustomerId).ToList();
        var customers = await _context.Customers.Where(x => customerIds.Contains(x.Id)).ToListAsync();
        var carCodes = details.Select(x => x.CarName).Distinct().ToList();
        var carDeliveries = await _context.CarDeliveries.Where(X => X.TableName == nameof(PlanningProduceProductDetail) && detailIds.Contains(X.TableId)).ToListAsync();

        var cars = carCodes.Select(x => new Car
        {
            Id = 0,
            LicensePlates = x
        });
        if (item.IsFinished)
        {
            var userIdFinished = await _procedureHelperService.GetUserFinish(item.ProcedureStatusId);
            itemOut.ShouldApproveGatePass = userIdFinished.Contains(userId);
        }
        itemOut.Items = new List<PlanningProduceProductCarGetDetailModel>();

        foreach (var car in cars)
        {
            var carPlanning = new PlanningProduceProductCarGetDetailModel
            {
                CarId = details.FirstOrDefault(x => x.CarName == car.LicensePlates)?.CarId,
                CarName = car.LicensePlates,
                Goods = new List<PlanningProduceProductGoodGetDetailModel>()
            };
            var goodDetails = details.Where(x => x.CarId == carPlanning.CarId && x.CarName == car.LicensePlates).ToList();
            carPlanning.ShouldExport = itemOut.IsFinished && goodDetails.All(x => x.ShouldExport)
                                && !string.IsNullOrEmpty(carDeliveries.FirstOrDefault(x => x.LicensePlates == car.LicensePlates).FileLink);
            carPlanning.IsCanceled = goodDetails.Any(x => x.IsCanceled);
            foreach (var itemDetail in goodDetails)
            {
                var goodDetail = new PlanningProduceProductGoodGetDetailModel
                {
                    Id = itemDetail.Id,
                    CustomerId = itemDetail.CustomerId,
                    Quantity = itemDetail.Quantity,
                    PlanningProduceProductId = id,
                    GoodsId = itemDetail.GoodsId,
                    UnitPrice = itemDetail.UnitPrice,
                    DiscountPrice = itemDetail.DiscountPrice,
                    TaxVAT = itemDetail.TaxVAT,
                    OrderProduceProductCode = itemDetail.OrderProduceProductCode,
                };

                if (!string.IsNullOrEmpty(itemDetail.FileDeliveredStr))
                {
                    goodDetail.FileDelivered = JsonConvert.DeserializeObject<FileDetailModel>(itemDetail.FileDeliveredStr);
                }

                var good = goods.FirstOrDefault(x => x.Id == itemDetail.GoodsId);
                if (good != null)
                {
                    goodDetail.GoodsCode = GoodNameGetter.GetCodeFromGood(good);
                    goodDetail.GoodsName = GoodNameGetter.GetNameFromGood(good);
                    goodDetail.GoodsNec = (good.Net ?? 0) * itemDetail.Quantity;
                    goodDetail.StockUnit = good.StockUnit;
                }
                goodDetail.CustomerName = customers.FirstOrDefault(x => x.Id == itemDetail.CustomerId)?.Name;

                var carDelivery = carDeliveries.FirstOrDefault(x => x.TableId == itemDetail.Id);
                if (carDelivery != null)
                {
                    carPlanning.LicensePlates = carDelivery.LicensePlates;
                    carPlanning.Note = carDelivery.Note;
                    carPlanning.FileLinks = JsonConvert.DeserializeObject<List<FileDetailModel>>(carDelivery.FileLink);
                }
                goodDetail.Promotions = await _billPromotionService.Get(id, nameof(PlanningProduceProduct),
                    carPlanning.CarId, carPlanning.CarName, itemDetail.CustomerId);
                if (carPlanning.Goods.Where(x => x.CustomerId == itemDetail.CustomerId).Sum(x => x.PromotionAmount) == 0)
                {
                    goodDetail.PromotionAmount = goodDetail.Promotions.Sum(x => x.Amount);
                }

                carPlanning.Goods.Add(goodDetail);
            }
            itemOut.Items.Add(carPlanning);
        }

        return itemOut;
    }

    public async Task<IEnumerable<PlanningProduceProductListGetterModel>> GetList(ProcedureForCreatePlanningProduct? procedureCode)
    {
        var createFromTable = nameof(OrderProduceProduct);
        if (procedureCode == ProcedureForCreatePlanningProduct.LEDGER_IMPORT)
        {
            createFromTable = nameof(LedgerProcedureProduct);
        }
        return await _context.PlanningProduceProducts.Where(x => !x.IsFinished).Select(x => _mapper.Map<PlanningProduceProductListGetterModel>(x)).ToListAsync();
    }

    public async Task Create(PlanningProduceProductModel form, int userId)
    {
        if (form.Items == null || form.Items.Count == 0)
        {
            throw new ErrorException(ErrorMessages.ItemsIsNull);
        }

        var produce = new PlanningProduceProduct
        {
            UserId = userId,
            Date = DateTime.Now,
            Note = form.Note,
            CreatedAt = DateTime.Now,
            UserCreated = userId,
            UserUpdated = userId,
            UpdatedAt = DateTime.Now,
            CreateFromTable = nameof(OrderProduceProduct),
        };
        if (form.ProcedureCode == ProcedureForCreatePlanningProduct.LEDGER_IMPORT)
        {
            produce.CreateFromTable = nameof(LedgerProcedureProduct);
        }
        if (string.IsNullOrEmpty(produce.ProcedureNumber))
        {
            produce.ProcedureNumber = await GetProcedureNumber();
        }

        using var scope = await _context.Database.BeginTransactionAsync();
        ProcedureStatusModelResponse status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.PLANNING_PRODUCE_PRODUCT));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        await _context.PlanningProduceProducts.AddAsync(produce);
        await _context.SaveChangesAsync();
        var customerId = form.Items.FirstOrDefault()?.CustomerId;
        var carId = form.Items.FirstOrDefault()?.CarId;
        string carCode = await _context.Cars.Where(x => x.Id == carId).Select(x => x.LicensePlates).FirstOrDefaultAsync();
        if (form.Items.Any(x => x.CarId == null))
        {
            var carNull = await _context.PlanningProduceProductDetails.Where(x => x.CarId == null).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
            int carIndex = 1;
            if (carNull != null && !string.IsNullOrEmpty(carNull.CarName))
            {
                carIndex = int.Parse(carNull.CarName.Split('-')[1]) + 1;
            }
            carCode = $"Xe ngoài-{carIndex}";
        }

        var orderProduceProduct = await ValidateWithOrderProduceProductAsync(form);
        var details = form.Items.Select(x => new PlanningProduceProductDetail
        {
            PlanningProduceProductId = produce.Id,
            GoodsId = x.GoodsId,
            Quantity = x.Quantity,
            CreatedAt = DateTime.Now,
            CustomerId = x.CustomerId,
            CarId = x.CarId,
            CarName = carCode,
            DiscountPrice = x.DiscountPrice,
            UnitPrice = x.UnitPrice,
            OrderProduceProductDetailId = x.Id,
            OrderProduceProductId = orderProduceProduct?.Id,
            OrderProduceProductCode = orderProduceProduct?.Code,
            CreateFromTable = produce.CreateFromTable
        });
        
        await _context.PlanningProduceProductDetails.AddRangeAsync(details);
        await _context.SaveChangesAsync();

        // add promotions
        await _billPromotionService.Create(form.BillPromotions, produce.Id, nameof(PlanningProduceProduct), carId, carCode, customerId);

        await _orderProduceProductService.SetIsProduced(form.Items.Select(X => X.Id));

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.PLANNING_PRODUCE_PRODUCT), status.Id, produce.Id, userId, produce.ProcedureNumber);
        await scope.CommitAsync();
    }
    private async Task<OrderProduceProduct> ValidateWithOrderProduceProductAsync(PlanningProduceProductModel form)
    {
        if (form.ProcedureCode == ProcedureForCreatePlanningProduct.LEDGER_IMPORT)
        {
            return null;
        }
        var orderProduceProductDetailIds = form.Items.Select(x => x.Id);
        var orderProduceProductDetailCheck = await _context.OrderProduceProductDetails.AnyAsync(x => orderProduceProductDetailIds.Contains(x.Id) && x.IsProduced);
        if (orderProduceProductDetailCheck)
        {
            throw new ErrorException("Hàng hoá đã lập kế hoạch");
        }

        var orderProduceProductDetailId = form.Items.FirstOrDefault()?.Id;
        var orderProduceProductDetail = await _context.OrderProduceProductDetails.FirstOrDefaultAsync(x => x.Id == orderProduceProductDetailId);
        var orderProduceProduct = await _context.OrderProduceProducts.FirstOrDefaultAsync(x => x.Id == orderProduceProductDetail.OrderProduceProductId);
        if (orderProduceProduct is null)
        {
            throw new ErrorException("Không tìm thấy đơn hàng mới");
        }
        if (!orderProduceProduct.IsFinished)
        {
            throw new ErrorException("Đơn hàng mới chưa hoàn thành");
        }
        return orderProduceProduct;
    }
    public async Task Update(PlanningProduceProductModel form, int userId)
    {
        if (form.Items == null || form.Items.Count == 0)
        {
            throw new ErrorException(ErrorMessages.ItemsIsNull);
        }

        var produce = await _context.PlanningProduceProducts.FindAsync(form.Id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }

        produce.UserId = userId;
        produce.UserUpdated = userId;
        produce.UpdatedAt = DateTime.Now;
        _context.PlanningProduceProducts.Update(produce);

        var customerId = form.Items.FirstOrDefault()?.CustomerId;
        var carId = form.Items.FirstOrDefault()?.CarId;
        var carName = form.Items.FirstOrDefault()?.CarName;
        if (carId < 0)
        {
            carId = null;
        }
        if (string.IsNullOrEmpty(carName))
        {
            carName = await _context.Cars.Where(x => x.Id == carId).Select(x => x.LicensePlates).FirstOrDefaultAsync();
            if (carId == null)
            {
                var carNulls = await _context.PlanningProduceProductDetails.Where(x => x.CarId == null).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                int carIndex = 1;
                if (carNulls != null)
                {
                    carIndex = int.Parse(carNulls.CarName.Split('-')[1]) + 1;
                }
                carName = $"Xe ngoài-{carIndex}";
            }
        }

        var goodIds = form.Items.Select(x => x.GoodsId).ToList();
        var detailGoodExists = await _context.PlanningProduceProductDetails.Where(x => x.PlanningProduceProductId == form.Id && x.CarId == carId && x.CarName == carName && goodIds.Contains(x.GoodsId)).ToListAsync();
        var createFromTable = nameof(OrderProduceProduct);
        OrderProduceProduct orderProduceProduct = null;
        if (form.ProcedureCode == ProcedureForCreatePlanningProduct.LEDGER_IMPORT)
        {
            createFromTable = nameof(LedgerProcedureProduct);
        }
        else
        {
            var orderProduceProductDetailId = form.Items.FirstOrDefault()?.Id;
            var orderProduceProductId = await _context.OrderProduceProductDetails.Where(x => x.Id == orderProduceProductDetailId).Select(x => x.OrderProduceProductId).FirstOrDefaultAsync();
            orderProduceProduct = await _context.OrderProduceProducts.FirstOrDefaultAsync(x => x.Id == orderProduceProductId);

            if (orderProduceProduct is null)
            {
                throw new ErrorException("Không tìm thấy đơn hàng mới");
            }
            if (!orderProduceProduct.IsFinished)
            {
                throw new ErrorException("Đơn hàng mới chưa hoàn thành");
            }
        }

        var detailUpdates = new List<PlanningProduceProductDetail>();

        

        foreach (var item in form.Items)
        {
            var detailGoodExist = detailGoodExists.FirstOrDefault(x => x.GoodsId == item.GoodsId && x.CustomerId == item.CustomerId && x.OrderProduceProductDetailId == item.Id);
            if (detailGoodExist is null)
            {
                var detailAdd = new PlanningProduceProductDetail
                {
                    PlanningProduceProductId = produce.Id,
                    GoodsId = item.GoodsId,
                    Quantity = item.Quantity,
                    CreatedAt = DateTime.Now,
                    CustomerId = item.CustomerId,
                    CarId = carId,
                    CarName = carName,
                    DiscountPrice = item.DiscountPrice,
                    UnitPrice = item.UnitPrice,
                    OrderProduceProductDetailId = item.Id,
                    OrderProduceProductId = orderProduceProduct?.Id,
                    OrderProduceProductCode = orderProduceProduct?.Code,
                    CreateFromTable = createFromTable,
                    Date =DateTime.Now,

                };
                await _context.PlanningProduceProductDetails.AddAsync(detailAdd);
                await _context.SaveChangesAsync();
            }
            else
            {
                detailGoodExist.Quantity += item.Quantity;
                detailUpdates.Add(detailGoodExist);
            }
        }

        _context.PlanningProduceProductDetails.UpdateRange(detailUpdates);
        await _context.SaveChangesAsync();
        // add promotions
        await _billPromotionService.Create(form.BillPromotions, produce.Id, nameof(PlanningProduceProduct), carId, carName, customerId);

        await _orderProduceProductService.SetIsProduced(form.Items.Select(X => X.Id));
    }

    public async Task UpdatePlanning(PlanningProduceProductGetDetailModel form, int userId)
    {
        if (form.Items == null || form.Items.Count == 0)
        {
            throw new ErrorException(ErrorMessages.ItemsIsNull);
        }

        var produce = await _context.PlanningProduceProducts.FindAsync(form.Id);
        var planningDetails = await _context.PlanningProduceProductDetails.Where(x => x.PlanningProduceProductId == form.Id).ToListAsync();

        if (produce.IsFinished)
        {
            await SetFileDelivery(form, planningDetails);
            produce.IsDone = await _context.PlanningProduceProductDetails.Where(x => x.PlanningProduceProductId == form.Id).AllAsync(x => !string.IsNullOrEmpty(x.FileDeliveredStr));
            if (!produce.IsDone && !produce.IsPart)
            {
                produce.IsPart = await _context.PlanningProduceProductDetails.Where(x => x.PlanningProduceProductId == form.Id).AnyAsync(x => !string.IsNullOrEmpty(x.FileDeliveredStr));
            }
            _context.PlanningProduceProducts.Update(produce);
            await _context.SaveChangesAsync();
            return;
        }
        else
        {
            var isUploadFile = form.Items.SelectMany(x => x.Goods).Any(x => x.FileDelivered != null);
            if (isUploadFile)
            {
                throw new ErrorException(ErrorMessages.ProcedureNotFinished);
            }
        }
        await RemovePlanningProduceProductDetailWhenUpdating(form, planningDetails);


        produce.UserId = userId;
        produce.Note = form.Note;
        produce.UserUpdated = userId;
        produce.UpdatedAt = DateTime.Now;
        _context.PlanningProduceProducts.Update(produce);

        await _context.SaveChangesAsync();
    }

    private async Task RemovePlanningProduceProductDetailWhenUpdating(PlanningProduceProductGetDetailModel form, List<PlanningProduceProductDetail> planningDetails)
    {
        var detailIds = form.Items.SelectMany(x => x.Goods).Select(x => x.Id);
        var detailDels = planningDetails.Where(x => x.PlanningProduceProductId == form.Id && !detailIds.Contains(x.Id)).ToList();
        if (detailDels.Any())
        {
            // check car
            var carDeliveries = await _carDeliveryService.GetListForTableName(nameof(PlanningProduceProductDetail), detailDels.Select(x => x.Id));
            if (carDeliveries.Any())
            {
                foreach (var carDelivery in carDeliveries)
                {
                    var detailDel = planningDetails.FirstOrDefault(x => x.Id == carDelivery.TableId);
                    var detailOther = planningDetails.FirstOrDefault(x => x.CarId == detailDel.CarId && x.CarName == detailDel.CarName);
                    await _carDeliveryService.ResetTableId(carDelivery, detailOther.Id);
                }
            }

            // check payment
            var payments = await _paymentProposalService.GetListForTableName(nameof(PlanningProduceProductDetail), detailDels.Select(x => x.Id));
            if (payments.Any())
            {
                foreach (var payment in payments)
                {
                    var detailDel = planningDetails.FirstOrDefault(x => x.Id == payment.TableId);
                    var detailOther = planningDetails.FirstOrDefault(x => x.CarId == detailDel.CarId && x.CarName == detailDel.CarName);
                    await _paymentProposalService.ResetTableId(payment, detailOther.Id);
                }
            }

            // order produce product
            var orderProduceDetailIds = detailDels.Select(x => x.OrderProduceProductDetailId).Distinct();
            var orderProduceDetails = await _context.OrderProduceProductDetails.Where(x => orderProduceDetailIds.Contains(x.Id)).ToListAsync();
            foreach (var orderProduceDetail in orderProduceDetails)
            {
                orderProduceDetail.QuantityInProgress -= detailDels.Where(x => x.OrderProduceProductDetailId == orderProduceDetail.Id).Sum(x => x.Quantity);
                orderProduceDetail.QuantityReal = orderProduceDetail.QuantityRequired - orderProduceDetail.QuantityInProgress - orderProduceDetail.QuantityDelivered;
                orderProduceDetail.IsProduced = false;
            }
            _context.OrderProduceProductDetails.UpdateRange(orderProduceDetails);

            _context.PlanningProduceProductDetails.RemoveRange(detailDels);
        }
        await _context.SaveChangesAsync();
    }
    private async Task SetFileDelivery(PlanningProduceProductGetDetailModel form, List<PlanningProduceProductDetail> planningDetails)
    {
        if (form.Items != null)
        {
            foreach (var item in form.Items)
            {
                if (item.Goods != null)
                {
                    foreach (var good in item.Goods)
                    {
                        var planningDetail = planningDetails.Find(x => x.Id == good.Id);

                        bool haveFileDelivered = good.FileDelivered != null && string.IsNullOrEmpty(planningDetail.FileDeliveredStr);
                        if (good.FileDelivered == null)
                        {
                            var planningDetailAddedFileDelivered = planningDetails.FirstOrDefault(x => x.CustomerId == good.CustomerId && !string.IsNullOrEmpty(x.FileDeliveredStr) && x.Id != good.Id
                                                                        && x.CarName == item.CarName && x.CarId == item.CarId);
                            if (planningDetailAddedFileDelivered != null)
                            {
                                planningDetail.FileDeliveredStr = planningDetailAddedFileDelivered.FileDeliveredStr;
                                haveFileDelivered = true;
                            }
                        }

                        if (good.FileDelivered != null)
                        {
                            planningDetail.FileDeliveredStr = JsonConvert.SerializeObject(good.FileDelivered);
                        }

                        if (haveFileDelivered)
                        {
                            _context.PlanningProduceProductDetails.Update(planningDetail);

                            var orderProduceDetail = await _context.OrderProduceProductDetails.FirstOrDefaultAsync(x => x.Id == planningDetail.OrderProduceProductDetailId);
                            orderProduceDetail.QuantityDelivered += planningDetail.Quantity;
                            orderProduceDetail.QuantityInProgress -= planningDetail.Quantity;
                            orderProduceDetail.QuantityReal = orderProduceDetail.QuantityRequired - orderProduceDetail.QuantityInProgress - orderProduceDetail.QuantityDelivered;

                            if (orderProduceDetail.QuantityDelivered >= orderProduceDetail.QuantityRequired)
                            {
                                orderProduceDetail.IsDone = true;
                            }
                            _context.OrderProduceProductDetails.Update(orderProduceDetail);
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();
            var orderProduceIds = planningDetails.Select(x => x.OrderProduceProductId).Where(x => x != null).Distinct();
            await UpdateStatusOrderProduceProductWhenUploadFileDelivered(orderProduceIds);
        }
    }

    private async Task UpdateStatusOrderProduceProductWhenUploadFileDelivered(IEnumerable<int?> ids)
    {
        foreach (var id in ids)
        {
            if (id == null) continue;

            var orderProduce = await _context.OrderProduceProducts.FirstOrDefaultAsync(x => x.Id == id);
            if (!orderProduce.IsDelivered)
            {
                orderProduce.IsDelivered = true;
            }

            if (!orderProduce.IsDone)
            {
                orderProduce.IsDone = await _context.OrderProduceProductDetails.AllAsync(x => x.OrderProduceProductId == id && x.IsDone);
            }

            _context.OrderProduceProducts.Update(orderProduce);
        }
        await _context.SaveChangesAsync();
    }

    public async Task Accept(int id, int userId, int year)
    {
        var produce = await _context.PlanningProduceProducts.FindAsync(id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }

        // validate condition
        var status = await _procedurePlanningProduceProductHelper.GetStatusAcceptForPlanningProduceProduct(produce.ProcedureStatusId ?? 0, id, userId, year);
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        if (!produce.IsPlanningProduct && (status.IsFinish || status.ProcedureConditionCode == nameof(ProcedureOrderProduceProductConditionEnum.PlanningWarehouse)))
        {
            produce.IsPlanningProduct = true;
            // create a produce product for warehouse
            await _warehouseProduceProductService.Create(id, userId);
        }

        if (status.IsFinish)
        {
            produce.IsFinished = status.IsFinish;
            produce.Code = await GetCodeAsync(produce.CreateFromTable);
            // update DeliveryCode in detail
            var details = await UpdateDeliveryCodeAsync(id);
            if (status.ProcedureConditionCode == nameof(ProcedureOrderProduceProductConditionEnum.SendToCashier))
            {
                await _procedurePlanningProduceProductHelper.SendToCashierAsync(details, id, userId, TranTypeConst.Paid, year, produce.Code);
            }
        }

        
        produce.UpdatedAt = DateTime.Now;

        _context.PlanningProduceProducts.Update(produce);
        await _context.SaveChangesAsync();

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.PLANNING_PRODUCE_PRODUCT), status.Id, userId, id, produce.ProcedureNumber);

        await _procedureHelperService.WriteProcedureLogToSendNotification(nameof(ProcedureEnum.PLANNING_PRODUCE_PRODUCT), status.Id, id, userId, produce.ProcedureNumber, status.IsFinish);
    }

    private async Task<IEnumerable<PlanningProduceProductDetail>> UpdateDeliveryCodeAsync(int id)
    {
        var details = await _context.PlanningProduceProductDetails.Where(x => x.PlanningProduceProductId == id && x.CreateFromTable == nameof(OrderProduceProduct)).ToListAsync();
        var deliveryCode = await _context.PlanningProduceProductDetails
            .Where(x => !string.IsNullOrEmpty(x.DeliveryCode) 
                            && x.Date.Month == DateTime.Today.Month && x.Date.Year == DateTime.Today.Year
                            && x.CreateFromTable == nameof(OrderProduceProduct))
            .OrderByDescending(x => x.DeliveryCode).ThenByDescending(x => x.Id).Select(x => x.DeliveryCode).FirstOrDefaultAsync();
        _logger.LogInformation("Delivery Code max {0} for produce {1}", deliveryCode, id);

        var customerIds = details.Select(x => x.CustomerId).Distinct().ToList();

        foreach (var customerId in customerIds)
        {
            var carCodes = details.Where(x => x.CustomerId == customerId).Select(x => x.CarName).Distinct();
            foreach (var carCode in carCodes)
            {
                var code = _procedureHelperService.GetProcedureNumber(deliveryCode, 4);
                var detailUpdates = details.Where(x => x.CarName == carCode && x.CustomerId == customerId).ToList();
                if (detailUpdates.Any())
                {
                    detailUpdates = detailUpdates.ConvertAll(x =>
                    {
                        x.Date = DateTime.Now;
                        x.DeliveryCode = code;
                        return x;
                    });
                    deliveryCode = code;
                }
            }
        }
        _context.PlanningProduceProductDetails.UpdateRange(details);
        await _context.SaveChangesAsync();
        return details;
    }
    public async Task Canceled(int id)
    {
        var item = await _context.PlanningProduceProducts.FindAsync(id);
        if (item == null)
        {
            throw new ErrorException(ErrorMessages.DataNotFound);
        }
        // check planning
        var existPlanningDetail = await _context.PlanningProduceProductDetails.AnyAsync(x => x.OrderProduceProductId == id);
        if (existPlanningDetail)
        {
            throw new ErrorException("Đã tồn tại kế hoạch sản xuất không thể xóa");
        }

        item.IsCanceled = true;
        _context.PlanningProduceProducts.Update(item);

        await _context.SaveChangesAsync();
    }

    private async Task<string> GetProcedureNumber()
    {
        var item = await _context.PlanningProduceProducts.OrderByDescending(x => x.Id).FirstOrDefaultAsync();
        var procedureNumber = _procedureHelperService.GetProcedureNumber(item?.ProcedureNumber);
        return $"KH-{procedureNumber}";
    }

    public async Task NotAccept(int id, int userId)
    {
        var produce = await _context.PlanningProduceProducts.FindAsync(id);
        if (produce.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }

        produce.NoteNotAccept = produce.ProcedureStatusName + "; " + produce.NoteNotAccept;
        // validate condition
        var status = await _procedureHelperService.GetStatusInit(nameof(ProcedureEnum.PLANNING_PRODUCE_PRODUCT));
        produce.ProcedureStatusId = status.Id;
        produce.ProcedureStatusName = status.P_StatusName;

        produce.UpdatedAt = DateTime.Now;
        _context.PlanningProduceProducts.Update(produce);

        await _procedureHelperService.WriteProcedureLog(nameof(ProcedureEnum.PLANNING_PRODUCE_PRODUCT), status.Id, userId, id, produce.ProcedureNumber, true);
        await _context.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var item = await _context.PlanningProduceProducts.FindAsync(id);
        if (item.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureFinished);
        }
        var isExistLog = await _procedureHelperService.ExistLog(id, nameof(ProcedureEnum.PLANNING_PRODUCE_PRODUCT));
        if (isExistLog)
        {
            throw new ErrorException(ErrorMessages.ProcedureCannotDelete);
        }
        if (item is null)
            throw new ErrorException(ErrorMessages.DataNotFound);

        var details = await _context.PlanningProduceProductDetails.Where(x => x.PlanningProduceProductId == id).ToListAsync();

        _context.PlanningProduceProductDetails.RemoveRange(details);
        _context.PlanningProduceProducts.Remove(item);
        await UpdateOrderProduceProduct(details);
        await _procedureHelperService.DeleteLog(id, nameof(ProcedureEnum.PLANNING_PRODUCE_PRODUCT));

        await _context.SaveChangesAsync();
    }

    public async Task SetCarDelivery(CarDeliveryModel carDelivery, int id)
    {
        // check name duplicate
        var planningDetailId = await _context.PlanningProduceProductDetails
                            .Where(X => X.PlanningProduceProductId == id && X.CarId == carDelivery.CarId && X.CarName == carDelivery.CarName)
                            .Select(x => x.Id)
                            .FirstOrDefaultAsync();

        await _carDeliveryService.Set(carDelivery, nameof(PlanningProduceProductDetail), planningDetailId);
    }

    public async Task<CarDeliveryModel> GetCarDelivery(int? carId, string carName, int id)
    {
        var planningDetails = await _context.PlanningProduceProductDetails
                           .Where(X => X.PlanningProduceProductId == id && X.CarId == carId && X.CarName == carName)
                           .ToListAsync();
        var planningDetailIds = planningDetails.Select(x => x.Id);
        var carDeliveryId = await _context.CarDeliveries.Where(X => X.TableName == nameof(PlanningProduceProductDetail) && planningDetailIds.Contains(X.TableId)).FirstOrDefaultAsync();

        var data = await _carDeliveryService.Get(carDeliveryId?.Id) ?? new CarDeliveryModel();
        data.CarId = carId;
        data.CarName = carName;

        return data;
    }

    public async Task<IEnumerable<CarGetterModel>> GetListCar(int id)
    {
        var cars = await _context.Cars.Select(x => _mapper.Map<CarGetterModel>(x)).ToListAsync();
        var carNames = await _context.PlanningProduceProductDetails.Where(x => x.PlanningProduceProductId == id && x.CarId == null).Select(X => X.CarName).Distinct().ToListAsync();
        int i = 0;
        foreach (var carName in carNames)
        {
            i--;
            cars.Add(new CarGetterModel
            {
                Id = i,
                LicensePlates = carName
            });
        }
        return cars;
    }

    public async Task SetPaymentProposal(PaymentProposalModel model, int? carId, string carName, int id, int userId)
    {
        var planningDetailId = await _context.PlanningProduceProductDetails
                            .Where(X => X.PlanningProduceProductId == id && X.CarId == carId && X.CarName == carName)
                            .Select(x => x.Id)
                            .FirstOrDefaultAsync();
        await _paymentProposalService.Create(model, userId, planningDetailId, nameof(PlanningProduceProductDetail));
    }

    public async Task<PaymentProposalModel> GetPaymentProposal(int? carId, string carName, int id)
    {
        var planningDetailIds = await _context.PlanningProduceProductDetails
                            .Where(X => X.PlanningProduceProductId == id && X.CarId == carId && X.CarName == carName)
                            .Select(x => x.Id)
                            .ToListAsync();
        var paymentProposalId = await _context.PaymentProposals.Where(X => X.TableName == nameof(PlanningProduceProductDetail) && planningDetailIds.Contains(X.TableId ?? 0)).FirstOrDefaultAsync();
        var data = await _paymentProposalService.GetDetail(paymentProposalId?.Id ?? 0) ?? new PaymentProposalModel();
        return data;
    }

    public async Task CancelPlanningDetail(int id, List<int> detailIds)
    {
        var planning = await _context.PlanningProduceProducts.FindAsync(id);
        if (planning.IsDone)
        {
            throw new ErrorException("Không thể hủy đơn hàng đã hoàn thành");
        }

        var planningDetails = await _context.PlanningProduceProductDetails.Where(x => detailIds.Contains(x.Id)).ToListAsync();
        if (planningDetails.Any(x => !string.IsNullOrEmpty(x.FileDeliveredStr)))
        {
            throw new ErrorException("Không thể hủy đơn hàng đã giao");
        }

        planningDetails = planningDetails.ConvertAll(x =>
        {
            x.IsCanceled = true;
            return x;
        });
        _context.PlanningProduceProductDetails.UpdateRange(planningDetails);
        await _context.SaveChangesAsync();

        planning.IsCanceled = await _context.PlanningProduceProductDetails.Where(x => x.PlanningProduceProductId == id).AllAsync(x => x.IsCanceled);
        planning.IsPart = !planning.IsCanceled && planning.IsFinished;
        _context.PlanningProduceProducts.Update(planning);
        await _context.SaveChangesAsync();

        await UpdateOrderProduceProduct(planningDetails);

    }

    public async Task AddLedger(int id, int carId, string carName, int year)
    {
        await _produceProductLedgerService.AddLedgerDebitFromPlanningProduct(id, carId, carName, year);
    }

    private async Task<string> GetCodeAsync(string createFromTable)
    {
        var month = DateTime.Today.Month;

        var codeNumber = await _context.PlanningProduceProducts
            .Where(x => x.IsFinished 
                        && x.Date.Month == month 
                        && x.CreateFromTable == createFromTable)
            .OrderByDescending(x => x.Id)
            .Select(x => x.Code)
            .FirstOrDefaultAsync();

        return _procedureHelperService.GetCode(codeNumber, "Planning");
    }

    private async Task UpdateOrderProduceProduct(List<PlanningProduceProductDetail> planningDetails)
    {
        var orderProduceDetailIds = planningDetails.Select(x => x.OrderProduceProductDetailId).ToList();
        var orderProduceDetails = await _context.OrderProduceProductDetails.Where(x => orderProduceDetailIds.Contains(x.Id)).ToListAsync();
        //
        foreach (var planningDetail in planningDetails)
        {
            var orderProduceDetail = orderProduceDetails.FirstOrDefault(x => x.Id == planningDetail.OrderProduceProductDetailId);
            if (orderProduceDetail != null)
            {
                orderProduceDetail.QuantityInProgress -= planningDetail.Quantity;
                orderProduceDetail.QuantityReal = orderProduceDetail.QuantityRequired - orderProduceDetail.QuantityInProgress - orderProduceDetail.QuantityDelivered;
                orderProduceDetail.IsProduced = orderProduceDetail.QuantityRequired <= orderProduceDetail.QuantityInProgress;
            }
        }
        _context.OrderProduceProductDetails.UpdateRange(orderProduceDetails);
        await _context.SaveChangesAsync();
    }

    public async Task UpDateShouldExportDetail(int id, int? carId, string carName, int userId)
    {
        var planning = await _context.PlanningProduceProducts.FindAsync(id);
        if (!planning.IsFinished)
        {
            throw new ErrorException(ErrorMessages.ProcedureNotFinished);
        }

        var planningDetails = await _context.PlanningProduceProductDetails
                                            .Where(x => x.PlanningProduceProductId == id
                                            && x.CarId == carId
                                            && x.CarName == carName).ToListAsync();
        planningDetails = planningDetails.ConvertAll(x =>
        {
            x.ShouldExport = true;
            return x;
        });
        _context.UpdateRange(planningDetails);
        await _context.SaveChangesAsync();
    }
}