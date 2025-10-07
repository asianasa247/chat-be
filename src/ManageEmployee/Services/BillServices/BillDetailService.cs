using AutoMapper;
using Common.Constants;
using Common.Errors;
using Common.Helpers;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.Entities.BillEntities;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.GoodsEntities;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Bills;
using ManageEmployee.Services.Interfaces.Invoices;
using ManageEmployee.Services.Interfaces.Ledgers;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ManageEmployee.Services.BillServices;


public class BillDetailService : IBillDetailService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILedgerService _ledgerService;
    private readonly IInvoiceCreator _invoiceCreator;
    private readonly ILedgerForSaleService _ledgerBillService;
    private readonly IBillPromotionService _billPromotionService;
    public BillDetailService(
        ApplicationDbContext context,
        IMapper mapper,
        ILedgerService ledgerService,
        IInvoiceCreator invoiceCreator,
        ILedgerForSaleService ledgerBillService,
        IBillPromotionService billPromotionService
    )
    {
        _context = context;
        _mapper = mapper;
        _ledgerService = ledgerService;
        _invoiceCreator = invoiceCreator;
        _ledgerBillService = ledgerBillService;
        _billPromotionService = billPromotionService;
    }

    public async Task<object> GetListBillDetailAndPromotion(int billId)
    {
        try
        {
            var billDetails = await GetListByBillId(billId);
            var promotions = await _billPromotionService.Get(billId, nameof(BillDetailRefund));
            if (promotions is null)
            {
                promotions = await _billPromotionService.Get(billId, nameof(Bill));
                promotions = promotions.ConvertAll(x =>
                {
                    x.Qty = 0;
                    x.Amount = 0;
                    return x;
                });
            }
            return new
            {
                billDetail = billDetails,
                promotions
            };
        }
        catch (Exception ex)
        {
            throw new ErrorException(ex.Message);
        }
    }

    public async Task<List<BillDetailViewPaging>> GetListByBillId(int billId)
    {
        try
        {
            var billDetails = await _context.BillDetails.Where(x => !x.IsDeleted && x.BillId == billId)
                .Select(x => _mapper.Map<BillDetailViewPaging>(x)).ToListAsync();
            if (!billDetails.Any())
            {
                throw new Exception("Không tìm thấy chi tiết đơn hàng");
            }

            var goodIds = billDetails.Select(x => x.GoodsId).ToList();
            List<Goods> goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();
            var billDetailIds = billDetails.Select(y => y.Id);
            var billDetailRefunds = await _context.BillDetailRefunds.Where(x => billDetailIds.Contains(x.BillDetailId ?? 0)).ToListAsync();

            foreach (var item in billDetails)
            {
                item.PricePay = item.Quantity * (item.UnitPrice + item.TaxVAT - item.DiscountPrice);
                var good = goods.Find(x => x.Id == item.GoodsId);
                if (good != null)
                {
                    item.GoodsName = GoodNameGetter.GetNameFromGood(good);
                    item.GoodsCode = GoodNameGetter.GetCodeFromGood(good);
                    item.WareHouseName = good.WarehouseName;
                    item.MinStockLevel = good.MinStockLevel;
                    item.MaxStockLevel = good.MaxStockLevel;
                    item.Image1 = good.Image1;
                    item.Detail1 = good.Detail1;
                    item.Detail2 = good.Detail2;
                    item.UnitName = good?.StockUnit;
                    var billRefund = billDetailRefunds.Find(x => x.BillDetailId == item.Id);
                    item.QuantityRefund = billRefund?.Quantity ?? 0;
                    item.NoteRefund = billRefund?.Note;
                }
            }
            // discount and Surcharge
            var bill = await _context.Bills.FindAsync(billId);
            if (bill != null)
            {
                billDetails[0].DiscountPriceBill = bill.DiscountPrice;
                billDetails[0].SurchargeBill = bill.Surcharge ?? 0;
            }
            return billDetails;
        }
        catch (Exception ex)
        {
            throw new ErrorException(ex.Message);
        }
    }

    public async Task<List<BillDetailViewPaging>> GetListByBillIdForWareHouse(int billId, int year)
    {
        var billDetails = await _context.BillDetails.Where(x => !x.IsDeleted && x.BillId == billId)
            .Select(x => _mapper.Map<BillDetailViewPaging>(x)).ToListAsync();
        var goodIds = billDetails.Select(x => x.GoodsId).ToList();
        List<Goods> goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();
        List<ChartOfAccount> accounts = await _context.GetChartOfAccount(year).Where(x => x.ParentRef.Contains("1561")).ToListAsync();
        List<SoChiTietViewModel> relations = await _context.GetLedger(year, 2)
            .Where(x => x.CreditCode == "1561" || x.DebitCode == "1561")
        .Select(k => new SoChiTietViewModel
        {
            DebitCode = k.DebitCode,
            DebitDetailCodeSecond = k.DebitDetailCodeSecond,
            DebitDetailCodeFirst = k.DebitDetailCodeFirst,
            CreditCode = k.CreditCode,
            CreditDetailCodeFirst = k.CreditDetailCodeFirst,
            CreditDetailCodeSecond = k.CreditDetailCodeSecond,
            CreditWarehouseCode = k.CreditWarehouse,
            DebitWarehouseCode = k.DebitWarehouse,
            NameGood = string.IsNullOrEmpty(k.DebitDetailCodeSecondName) ? k.DebitDetailCodeFirstName : k.DebitDetailCodeSecondName,
            Quantity = k.Quantity,
            Amount = k.Amount,
        })
        .OrderBy(x => x.DebitCode)
        .ToListAsync();

        var listStorege = await _context.GetChartOfAccount(year).Where(x => (x.Classification == 2 || x.Classification == 3) && !x.HasChild).ToListAsync();
        var shelves = await _context.WareHouseShelves.ToListAsync();
        var floors = await _context.WareHouseFloors.ToListAsync();
        var positions = await _context.WareHousePositions.ToListAsync();
        foreach (var item in billDetails)
        {
            if (item.Suggestions == null)
                item.Suggestions = new List<BillDetailQrInfo>();

            item.PricePay = item.Quantity * (item.UnitPrice + item.TaxVAT) - item.DiscountPrice;
            var good = goods.FirstOrDefault(x => x.Id == item.GoodsId);

            if (good != null)
            {
                item.GoodsName = GoodNameGetter.GetNameFromGood(good);
                item.GoodsCode = GoodNameGetter.GetCodeFromGood(good);
                item.WareHouseName = good.WarehouseName;
                item.MinStockLevel = good.MinStockLevel;
                item.MaxStockLevel = good.MaxStockLevel;
                item.Image1 = good.Image1;
                ChartOfAccount account = accounts.FirstOrDefault(x => x.Code == item.GoodsCode && (string.IsNullOrEmpty(good.Warehouse) || x.WarehouseCode == good.Warehouse));
                if (account != null)
                {
                    item.OpenQuantity = account.OpeningStockQuantity;
                    item.InputQuantity = relations.Where(x => x.DebitDetailCodeFirst == good.Detail1
                                                        && x.DebitCode == good.Account
                                                      && (string.IsNullOrEmpty(good.Detail2) || x.DebitDetailCodeSecond == good.Detail2)
                                                      && (string.IsNullOrEmpty(account.WarehouseCode) || x.DebitWarehouseCode == account.WarehouseCode)).Sum(x => x.Quantity);
                    item.OutputQuantity = relations.Where(x => x.CreditDetailCodeFirst == good.Detail1
                                                        && x.CreditCode == good.Account
                                                    && (string.IsNullOrEmpty(good.Detail2) || x.CreditDetailCodeSecond == good.Detail2)
                                                    && (string.IsNullOrEmpty(account.WarehouseCode) || x.CreditWarehouseCode == account.WarehouseCode)).Sum(x => x.Quantity);
                    item.CloseQuantity = item.OpenQuantity + item.InputQuantity - item.OutputQuantity;
                }

                // Get Unit Name
                ChartOfAccount storage;
                if (!string.IsNullOrEmpty(good.Detail2))
                {
                    string parentRef = good.Account + ":" + good.Detail1;
                    storage = listStorege.FirstOrDefault(x => x.Code == good.Detail2 && x.ParentRef == parentRef &&
                            (string.IsNullOrEmpty(good.Warehouse) || x.WarehouseCode == good.Warehouse));
                }
                else if (!string.IsNullOrEmpty(good.Detail1))
                    storage = listStorege.FirstOrDefault(x => x.Code == good.Detail1 && x.ParentRef == good.Account && (string.IsNullOrEmpty(good.Warehouse) || x.WarehouseCode == good.Warehouse));
                else
                    storage = listStorege.FirstOrDefault(x => x.Code == good.Account);

                item.UnitName = storage?.StockUnit ?? "";

                var goodWarehouses = await _context.GoodWarehouses.Where(x => x.Quantity > 0
                                    && x.Account == good.Account
                                    && (string.IsNullOrEmpty(good.Detail1) || x.Detail1 == good.Detail1)
                                    && (string.IsNullOrEmpty(good.Detail2) || x.Detail2 == good.Detail2)
                                    && (string.IsNullOrEmpty(good.Warehouse) || x.Warehouse == good.Warehouse))
                                        .OrderBy(x => x.DateExpiration).ToListAsync();
                if (!goodWarehouses.Any())
                    continue;

                double quatity = item.Quantity;
                foreach (var goodWarehouse in goodWarehouses)
                {
                    var goodWarehouseDetails = await _context.GoodWarehousesPositions.Where(x => x.Quantity > 0
                                    && x.GoodWarehousesId == goodWarehouse.Id)
                                        .OrderBy(x => x.Quantity).ToListAsync();
                    var position = "";
                    List<PositionBillDetail> positionDetails = new();
                    foreach (var goodWarehouseDetail in goodWarehouseDetails)
                    {
                        var positionBillDetail = shelves.FirstOrDefault(x => x.Id == goodWarehouseDetail.WareHouseShelvesId)?.Name + " - "
                                + floors.FirstOrDefault(x => x.Id == goodWarehouseDetail.WareHouseFloorId)?.Name + " - "
                                + positions.FirstOrDefault(x => x.Id == goodWarehouseDetail.WareHousePositionId)?.Name + "; ";
                        var positionDetail = new PositionBillDetail()
                        {
                            Id = goodWarehouseDetail.Id,
                            Position = positionBillDetail,
                            Quantity = goodWarehouseDetail.Quantity,
                            QuantityReal = 0
                        };
                        positionDetails.Add(positionDetail);
                        position += positionBillDetail;
                    }
                    BillDetailQrInfo suggestion = new()
                    {
                        DateExpiration = goodWarehouse.DateExpiration,
                        Order = goodWarehouse.Order ?? 0,
                        Position = position,
                        Quantity = goodWarehouse.Quantity,
                        QrCode = item.GoodsCode + " " + goodWarehouse.Order + "-" + goodWarehouse.Id,
                        Positions = positionDetails
                    };
                    if (quatity > 0)
                    {
                        suggestion.IsVisible = true;
                        quatity -= goodWarehouse.Quantity;
                    }
                    item.Suggestions.Add(suggestion);
                }
                item.GoodWareHouseIds = goodWarehouses.Select(x => x.Id).ToList();
            }
        }
        return billDetails;
    }

    public async Task<List<BillDetail>> Create(List<BillDetailModel> requests, int year)
    {
        try
        {
            if (!requests.Any())
            {
                throw new ErrorException(ErrorMessage.DATA_IS_EMPTY);
            }

            //delete old bill detail
            var billDetails = await _context.BillDetails.Where(x => x.BillId == requests[0].BillId).AsNoTracking().ToListAsync();
            if (billDetails.Count > 0)
            {
                _context.BillDetails.RemoveRange(billDetails);
            }

            List<BillDetail> listBillDetail = new List<BillDetail>();

            var billId = requests[0].BillId;
            var branchId = await _context.Bills.Where(x => x.Id == requests[0].BillId).Select(x => x.BranchId).FirstOrDefaultAsync();
            
            foreach (var request in requests)
            {
                var current = await _context.Bills.Where(x => x.Id == request.BillId).FirstOrDefaultAsync();
                if (current == null) continue;
                BillDetail billDetailModel = new BillDetail
                {
                    Id = 0,
                    BillId = request.BillId,
                    GoodsId = request.GoodsId,
                    Quantity = request.Quantity,
                    UnitPrice = request.UnitPrice ?? 0,
                    DiscountPrice = current.DiscountPrice,
                    DiscountType = !string.IsNullOrEmpty(current.DiscountType) ? current.DiscountType : "money",
                    TaxVAT = request.TaxVAT ?? 0,
                    Note = request.Note,
                    DateExpiration = request.DateExpiration,
                    DateManufacture = request.DateManufacture,
                    DeliveryCode = request.DeliveryCode,
                    BranchId = branchId,
                    LotNumber= request.LotNumber
                };
                listBillDetail.Add(billDetailModel);
            }

            await _context.BillDetails.AddRangeAsync(listBillDetail);
            var listBillTracking = await _context.BillTrackings.Where(x => x.TranType == TranTypeConst.Paid).ToListAsync();
            if (listBillTracking.Count > 10)
            {
                listBillTracking.RemoveRange(listBillTracking.Count - 10, 10);
                _context.BillTrackings.RemoveRange(listBillTracking);
            }
            await _context.SaveChangesAsync();

            // hoa don da thanh toan moi update ke toan
            var isBillPaied = await _context.BillTrackings.Where(x => x.TranType.Contains(TranTypeConst.Paid) && x.BillId == billId).AnyAsync();
            if (isBillPaied)
            {
                await _ledgerBillService.AddLedgerBill(listBillDetail, year);
                await _invoiceCreator.PerformAsync(billId);
            }

            // save promotion
            var billPromotions = await _context.BillPromotions.Where(x => x.TableId == billId && x.TableName == nameof(Bill)).ToListAsync();
            if (billPromotions.Any() && isBillPaied)
            {
                // save in ledger
                var bill = await _context.Bills.FindAsync(billId);
                var customer = await _context.Customers.FindAsync(bill?.CustomerId);
                var customer_tax = await _context.CustomerTaxInformations.FirstOrDefaultAsync(x => x.CustomerId == bill.CustomerId);

                int monthLed = DateTime.Today.Month;
                string orginalVoucherNumber = "";
                string voucherMonth = monthLed < 10 ? "0" + monthLed : monthLed.ToString();
                var maxOriginalVoucher = 0;
                var ledgerExist = await _context.GetLedger(year, bill.IsPrintBill ? 1 : 3).AsNoTracking().Where(x => !x.IsDelete && x.Type == "CN"
                                                           && x.OrginalBookDate.Value.Year == DateTime.Today.Year && x.OrginalBookDate.Value.Month == DateTime.Today.Month).ToListAsync();

                if (ledgerExist != null && ledgerExist.Count > 0)
                {
                    maxOriginalVoucher = ledgerExist.Max(x => int.Parse(x.OrginalVoucherNumber.Split('-').Last()));
                }
                maxOriginalVoucher++;
                var orderString = LedgerHelper.GetOriginalVoucher(maxOriginalVoucher);
                orginalVoucherNumber = $"CN{voucherMonth}-{DateTime.Today.Year.ToString().Substring(2, 2)}-{orderString}";
                var billDate = bill.Date ?? DateTime.Today;
                billDate = new DateTime(billDate.Year, billDate.Month, billDate.Day);
                var deliveryCode = listBillDetail.FirstOrDefault()?.DeliveryCode;
                foreach (var billPromotion in billPromotions)
                {
                    var ledger = new Ledger
                    {
                        Type = "CN",
                        OrginalCompanyName = customer?.Name,
                        OrginalAddress = customer?.Address,
                        OrginalDescription = billPromotion.Note,
                        AttachVoucher = string.IsNullOrEmpty(deliveryCode) ? bill.BillNumber : deliveryCode,
                        DebitCode = billPromotion.Account,
                        DebitCodeName = billPromotion.AccountName,
                        DebitDetailCodeFirst = billPromotion.Detail1,
                        DebitDetailCodeFirstName = billPromotion.Detail1Name,
                        DebitDetailCodeSecond = billPromotion.Detail2,
                        DebitDetailCodeSecondName = billPromotion.Detail2Name,

                        CreditCode = customer?.DebitCode,
                        CreditDetailCodeFirst = customer?.DebitDetailCodeFirst,
                        CreditDetailCodeSecond = customer?.DebitDetailCodeSecond,
                        Amount = billPromotion.Amount,
                        IsInternal = bill.IsPrintBill ? 1 : 3,

                        BillId = billId,
                        Month = monthLed,
                        BookDate = billDate,
                        VoucherNumber = voucherMonth + "/CN",
                        OrginalVoucherNumber = orginalVoucherNumber,
                        Order = maxOriginalVoucher,
                        OrginalBookDate = billDate,
                        ReferenceBookDate = billDate,
                        InvoiceDate = billDate,
                        CreateAt = billDate,
                        ProjectCode = bill.ProjectCode,
                    };
                    if(ledger.DebitCode != null && ledger.CreditCode != null)
                    {
                        await _ledgerService.Create(ledger, year);
                    }
                }
            }
            return listBillDetail;
        }
        catch (Exception ex)
        {
            throw new ErrorException(ex.Message.ToString());
        }
    }

    public async Task<List<BillDetail>> UpdateNote(List<BillDetailNoteRequestModel> requests)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();
            List<BillDetail> results = new List<BillDetail>();
            for (int i = 0; i < requests.Count; i++)
            {
                BillDetail billDetail = await _context.BillDetails.Where(x => x.Id == requests[i].Id).FirstOrDefaultAsync();
                billDetail.Note = requests[i].Note;
                _context.BillDetails.Update(billDetail);
                results.Add(billDetail);
            }
            await _context.SaveChangesAsync();
            _context.Database.CommitTransaction();

            return results;
        }
        catch
        {
            _context.Database.RollbackTransaction();
            throw;
        }
    }

    public async Task Delete(int id)
    {
        var billDetail = await _context.BillDetails.FindAsync(id);
        if (billDetail != null)
        {
            billDetail.IsDeleted = true;
            _context.BillDetails.Update(billDetail);
            await _context.SaveChangesAsync();
        }
    }

    public async Task RefundGoodsAsync(BillRefundModel billRefund, int billId, int year)
    {
        var isValidate = billRefund.BillDetails.All(x => x.QuantityRefund == 0);
        if (isValidate)
        {
            throw new ErrorException("Bạn chưa chọn số lượng hoàn");
        }
        var bill = await _context.Bills.FindAsync(billId);
        var billDetailIds = billRefund.BillDetails.Select(x => x.Id).ToList();
        var billDetailStoreds = await _context.BillDetails.Where(x => billDetailIds.Contains(x.Id)).ToListAsync();
        var billDetailRefunds = new List<BillDetailRefund>();
        foreach (var billDetailStored in billDetailStoreds)
        {
            var billDetail = billRefund.BillDetails.Find(x => x.Id == billDetailStored.Id);
            if (billDetail.QuantityRefund > billDetailStored.Quantity)
            {
                throw new ErrorException("Quá số lượng hoàn");
            }
            billDetailStored.Status = "refund";

            billDetailStored.Quantity = billDetailStored.Quantity - (billDetail?.QuantityRefund ?? 0);
            var billDetailRefund = new BillDetailRefund
            {
                BillDetailId = billDetailStored.Id,
                BillId = billDetailStored.BillId,
                Quantity = billDetail.QuantityRefund,
                CreatedAt = DateTime.Now,
                Note = billDetail.NoteRefund,
                GoodsId = billDetailStored.GoodsId,
                CreatedBy = 0,
                UnitPrice = billDetailStored.UnitPrice,
                BranchId = billDetailStored.BranchId,
            };
            billDetailRefunds.Add(billDetailRefund);
        }

        if (billRefund.Promotion != null)
        {
            var promotions = billRefund.Promotion.Where(x => x.Qty > 0).ToList();
            await _billPromotionService.Create(promotions, billId, nameof(BillDetailRefund));
        }

        // update amount in bill
        var amountRefund = billDetailStoreds
                            .Join(billRefund.BillDetails,
                            s => s.Id,
                            b => b.Id,
                            (s, b) => new { stored = s, refund = b })
                            .Sum(x => x.stored.UnitPrice * x.refund.QuantityRefund);

        bill.AmountRefund = amountRefund;
        // update ledger
        var goodIds = billDetailStoreds.Select(x => x.GoodsId).ToList();
        var goods = await _context.Goods.Where(x => goodIds.Contains(x.Id)).ToListAsync();

        var goodExports = await _context.GoodWarehouseExport.Where(x => x.BillId == billId).ToListAsync();
        foreach (var good in goods)
        {
            var billDetailRefund = billRefund.BillDetails.FirstOrDefault(x => x.GoodsId == good.Id);
            var goodExportCheck = goodExports.FirstOrDefault(x => x.GoodId == good.Id);
            if (goodExportCheck != null)
            {
                if (goodExportCheck.Quantity == billDetailRefund?.QuantityRefund)
                {
                    _context.GoodWarehouseExport.Remove(goodExportCheck);
                }
                else
                {
                    goodExportCheck.Quantity -= billDetailRefund?.QuantityRefund ?? 0;
                    _context.GoodWarehouseExport.Update(goodExportCheck);
                }
            }
        }
        await UpdateLedger(billRefund, goods, billId, year);
        // delete promotion
        await UpdatePromotionRefund(billRefund, billId, year);

        await _context.SaveChangesAsync();
    }

    private async Task UpdateLedger(BillRefundModel billRefund, IEnumerable<Goods> goods, int billId, int year)
    {
        var ledgers = await _context.Ledgers.Where(x => x.IsInternal != LedgerInternalConst.LedgerTemporary && x.BillId == billId).ToListAsync();

        foreach (var good in goods)
        {
            var ledgerChecks = ledgers.Where(x => (string.IsNullOrEmpty(x.CreditWarehouse) || x.CreditWarehouse == good.Warehouse)
                                && (string.IsNullOrEmpty(x.CreditDetailCodeFirst) || x.CreditDetailCodeFirst == good.Detail1)
                                && (string.IsNullOrEmpty(x.CreditDetailCodeSecond) || x.CreditDetailCodeSecond == good.Detail2));

            var billDetailRefund = billRefund.BillDetails.FirstOrDefault(x => x.GoodsId == good.Id);
            if (billDetailRefund?.QuantityRefund == null || billDetailRefund?.QuantityRefund == 0)
                continue;
            foreach (var ledgerCheck in ledgerChecks)
            {
                string ledgerString = JsonSerializer.Serialize(ledgerCheck);
                Ledger? ledgerUpdate = ledgerString.Deserialize<Ledger>();
                if (ledgerUpdate == null)
                {
                    continue;
                }

                ledgerUpdate.Id = 0;
                ledgerUpdate.Quantity = billDetailRefund?.QuantityRefund ?? 0;
                ledgerUpdate.Amount = ledgerUpdate.Quantity * ledgerUpdate.UnitPrice;
                ledgerUpdate.OrginalDescription = billDetailRefund?.NoteRefund;
                ledgerUpdate.CreditCode = ledgerCheck.DebitCode;
                ledgerUpdate.CreditCodeName = ledgerCheck.DebitCodeName;
                ledgerUpdate.CreditDetailCodeFirst = ledgerCheck.DebitDetailCodeFirst;
                ledgerUpdate.CreditDetailCodeFirstName = ledgerCheck.DebitDetailCodeFirstName;
                ledgerUpdate.CreditDetailCodeSecond = ledgerCheck.DebitDetailCodeSecond;
                ledgerUpdate.CreditDetailCodeSecondName = ledgerCheck.DebitDetailCodeSecondName;

                ledgerUpdate.DebitCode = ledgerCheck.CreditCode;
                ledgerUpdate.DebitCodeName = ledgerCheck.CreditCodeName;
                ledgerUpdate.DebitDetailCodeFirst = ledgerCheck.CreditDetailCodeFirst;
                ledgerUpdate.DebitDetailCodeFirstName = ledgerCheck.CreditDetailCodeFirstName;
                ledgerUpdate.DebitDetailCodeSecond = ledgerCheck.CreditDetailCodeSecond;
                ledgerUpdate.DebitDetailCodeSecondName = ledgerCheck.CreditDetailCodeSecondName;
                if (ledgerUpdate.DebitCode != null && ledgerUpdate.CreditCode != null)
                {
                    await _ledgerService.Create(ledgerUpdate, year);
                }
            }
        }
    }

    private async Task UpdatePromotionRefund(BillRefundModel billRefund, int billId, int year)
    {
        if (billRefund.Promotion == null)
        {
            return;
        }
        List<BillPromotionModel> billPromotions = billRefund.Promotion;
        var bill = await _context.Bills.FindAsync(billId);
        var customer = await _context.Customers.FindAsync(bill?.CustomerId);
        var customer_tax = await _context.CustomerTaxInformations.FirstOrDefaultAsync(x => x.CustomerId == bill.CustomerId);

        int monthLed = DateTime.Today.Month;
        string orginalVoucherNumber = "";
        string voucherMonth = monthLed < 10 ? "0" + monthLed : monthLed.ToString();
        var maxOriginalVoucher = 0;
        var ledgerExist = await _context.GetLedger(year, bill.IsPrintBill ? 1 : 3).AsNoTracking().Where(x => !x.IsDelete && x.Type == "CN"
                                                   && x.OrginalBookDate.Value.Year == DateTime.Today.Year && x.OrginalBookDate.Value.Month == DateTime.Today.Month).ToListAsync();

        if (ledgerExist != null && ledgerExist.Count > 0)
        {
            maxOriginalVoucher = ledgerExist.Max(x => int.Parse(x.OrginalVoucherNumber.Split('-').Last()));
        }
        maxOriginalVoucher++;
        var orderString = LedgerHelper.GetOriginalVoucher(maxOriginalVoucher);
        orginalVoucherNumber = $"CN{voucherMonth}-{DateTime.Today.Year.ToString().Substring(2, 2)}-{orderString}";
        var billDate = bill.Date ?? DateTime.Today;
        billDate = new DateTime(billDate.Year, billDate.Month, billDate.Day);
        var goodsPromotionIds = billPromotions.Select(x => x.Id);
        var goodsPromotionDetails = await _context.GoodsPromotionDetails.Where(x => goodsPromotionIds.Contains(x.GoodsPromotionId)).ToListAsync();


        foreach (var billPromotion in billPromotions)
        {
            var promotion = goodsPromotionDetails.FirstOrDefault(x => x.Id == billPromotion.Id);
            if (promotion is null)
                continue;

            var ledger = new Ledger
            {
                Type = "CN",
                OrginalCompanyName = customer?.Name,
                OrginalAddress = customer?.Address,
                OrginalDescription = billPromotion.Note,
                AttachVoucher = bill.BillNumber,
                CreditCode = promotion.Account,
                CreditDetailCodeFirst = promotion.Detail1,
                CreditDetailCodeSecond = promotion.Detail2,

                DebitCode = customer?.DebitCode,
                DebitDetailCodeFirst = customer?.DebitDetailCodeFirst,
                DebitDetailCodeSecond = customer?.DebitDetailCodeSecond,


                Amount = billPromotion.Amount,
                IsInternal = bill.IsPrintBill ? 1 : 3,

                BillId = billId,
                Month = monthLed,
                BookDate = billDate,
                VoucherNumber = voucherMonth + "/CN",
                OrginalVoucherNumber = orginalVoucherNumber,
                Order = maxOriginalVoucher,
                OrginalBookDate = billDate,
                ReferenceBookDate = billDate,
                InvoiceDate = billDate,
                CreateAt = billDate,
            };
            if (ledger.DebitCode != null && ledger.CreditCode != null)
            {
                await _ledgerService.Create(ledger, year);
            }
        }
    }
}