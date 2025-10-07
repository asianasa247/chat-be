using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using ManageEmployee.Services.Interfaces.Ledgers;
using ManageEmployee.Services.Interfaces.Ledgers.V2;
using ManageEmployee.Services.Interfaces.Ledgers.V3;
using ManageEmployee.Services.Interfaces.Goods;
using ManageEmployee.DataTransferObject.V2;
using ManageEmployee.DataTransferObject.V3;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Entities.ChartOfAccountEntities;
using Newtonsoft.Json;

namespace ManageEmployee.Services.LedgerServices.V3;

public class LedgerV3Service : ILedgerV3Service
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IChartOfAccountService _chartOfAccountService;
    private readonly ILedgerFixedAssetService _ledgerFixedAssetService;
    private readonly IChartOfAccountV2Service _chartOfAccountV2Service;
    private readonly IFixedAssetsService _fixedAssetsService;
    private readonly IFixedAssets242Service _fixedAssets242Service;
    private readonly ILedgerUpdateChartOfAccountNameService _ledgerUpdateChartOfAccountNameService;
    private string _operator = "+";
    private readonly IGoodWarehousesService _goodWarehousesService;

    public LedgerV3Service(ApplicationDbContext context, IMapper mapper, IChartOfAccountService chartOfAccountService, ILedgerFixedAssetService ledgerFixedAssetService, IChartOfAccountV2Service chartOfAccountV2Service, IFixedAssetsService fixedAssetsService, IFixedAssets242Service fixedAssets242Service, ILedgerUpdateChartOfAccountNameService ledgerUpdateChartOfAccountNameService, IGoodWarehousesService goodWarehousesService)
    {
        _context = context;
        _mapper = mapper;
        _chartOfAccountService = chartOfAccountService;
        _ledgerFixedAssetService = ledgerFixedAssetService;
        _chartOfAccountV2Service = chartOfAccountV2Service;
        _fixedAssetsService = fixedAssetsService;
        _fixedAssets242Service = fixedAssets242Service;
        _ledgerUpdateChartOfAccountNameService = ledgerUpdateChartOfAccountNameService;
        _goodWarehousesService = goodWarehousesService;
    }

    private async Task UpdateArsingChartOfAccount(List<Ledger> entities, int isInternal, int year, List<Ledger> ledgerUpdates = null)
    {
        var accounts = entities.Select(x => x.DebitCode).Concat(entities.Select(x => x.CreditCode)).Distinct();
        var warehouses = entities.Select(x => x.CreditWarehouse).Concat(entities.Select(x => x.DebitWarehouse)).Distinct().ToList();
        warehouses = warehouses.ConvertAll(x =>
        {
            if (string.IsNullOrEmpty(x))
            {
                x = string.Empty;
            }
            return x;
        });

        if (ledgerUpdates is null)
            ledgerUpdates = new List<Ledger>();

        foreach (var account in accounts)
        {
            var creditAmount = entities.Where(x => x.CreditCode == account).Sum(x => x.Amount);
            var creditQuantity = entities.Where(x => x.CreditCode == account).Sum(x => x.Quantity);

            var debitAmount = entities.Where(x => x.DebitCode == account).Sum(x => x.Amount);
            var debitQuantity = entities.Where(x => x.DebitCode == account).Sum(x => x.Quantity);

            var creditAmountUpdate = ledgerUpdates.Where(x => x.CreditCode == account).Sum(x => x.Amount);
            var creditQuantityUpdate = ledgerUpdates.Where(x => x.CreditCode == account).Sum(x => x.Quantity);

            var debitAmountUpdate = ledgerUpdates.Where(x => x.DebitCode == account).Sum(x => x.Amount);
            var debitQuantityUpdate = ledgerUpdates.Where(x => x.DebitCode == account).Sum(x => x.Quantity);

            await UpdateChartOfAccount(account, creditAmount - creditAmountUpdate, creditQuantity - creditQuantityUpdate,
                debitAmount - debitAmountUpdate, debitQuantity - debitQuantityUpdate, year, "", "", isInternal);

            var detail1s = entities.Where(x => x.CreditCode == account || x.DebitCode == account).Select(x => x.DebitDetailCodeFirst).Concat(entities.Select(x => x.CreditDetailCodeFirst)).Distinct();
            foreach (var detail1 in detail1s)
            {
                foreach (var warehouse in warehouses)
                {
                    var ledgerWareHouse = entities.Where(x => warehouse == string.Empty || x.DebitWarehouse == warehouse || x.CreditWarehouse == warehouse);
                    var ledgerWareHouseUpdate = ledgerUpdates.Where(x => warehouse == string.Empty || x.DebitWarehouse == warehouse || x.CreditWarehouse == warehouse);

                    var creditAmount1 = ledgerWareHouse.Where(x => x.CreditCode == account && x.CreditDetailCodeFirst == detail1 && (warehouse == string.Empty || x.CreditWarehouse == warehouse)).Sum(x => x.Amount);
                    var creditQuantity1 = ledgerWareHouse.Where(x => x.CreditCode == account && x.CreditDetailCodeFirst == detail1 && (warehouse == string.Empty || x.CreditWarehouse == warehouse)).Sum(x => x.Quantity);

                    var debitAmount1 = ledgerWareHouse.Where(x => x.DebitCode == account && x.DebitDetailCodeFirst == detail1 && (warehouse == string.Empty || x.DebitWarehouse == warehouse)).Sum(x => x.Amount);
                    var debitQuantity1 = ledgerWareHouse.Where(x => x.DebitCode == account && x.DebitDetailCodeFirst == detail1 && (warehouse == string.Empty || x.DebitWarehouse == warehouse)).Sum(x => x.Quantity);

                    var creditAmount1Update = ledgerWareHouseUpdate.Where(x => x.CreditCode == account && x.CreditDetailCodeFirst == detail1 && (warehouse == string.Empty || x.CreditWarehouse == warehouse)).Sum(x => x.Amount);
                    var creditQuantity1Update = ledgerWareHouseUpdate.Where(x => x.CreditCode == account && x.CreditDetailCodeFirst == detail1 && (warehouse == string.Empty || x.CreditWarehouse == warehouse)).Sum(x => x.Quantity);

                    var debitAmount1Update = ledgerWareHouseUpdate.Where(x => x.DebitCode == account && x.DebitDetailCodeFirst == detail1 && (warehouse == string.Empty || x.DebitWarehouse == warehouse)).Sum(x => x.Amount);
                    var debitQuantity1Update = ledgerWareHouseUpdate.Where(x => x.DebitCode == account && x.DebitDetailCodeFirst == detail1 && (warehouse == string.Empty || x.DebitWarehouse == warehouse)).Sum(x => x.Quantity);


                    await UpdateChartOfAccount(detail1, creditAmount1 - creditAmount1Update, creditQuantity1 - creditQuantity1Update,
                        debitAmount1 - debitAmount1Update, debitQuantity1 - debitQuantity1Update, year, account, warehouse, isInternal);

                    var detail2s = ledgerWareHouse.Where(x => (x.CreditCode == account || x.DebitCode == account)
                        && (x.DebitDetailCodeFirst == detail1 || x.CreditDetailCodeFirst == detail1))
                        .Select(x => x.DebitDetailCodeSecond).Concat(entities.Select(x => x.CreditDetailCodeSecond)).Distinct();
                    foreach (var detail2 in detail2s)
                    {
                        var creditAmount2 = ledgerWareHouse.Where(x => x.CreditCode == account && x.CreditDetailCodeFirst == detail1
                                        && (warehouse == string.Empty || x.CreditWarehouse == warehouse) && x.CreditDetailCodeSecond == detail2).Sum(x => x.Amount);
                        var creditQuantity2 = ledgerWareHouse.Where(x => x.CreditCode == account && x.CreditDetailCodeFirst == detail1
                                        && (warehouse == string.Empty || x.CreditWarehouse == warehouse) && x.CreditDetailCodeSecond == detail2).Sum(x => x.Quantity);

                        var debitAmount2 = ledgerWareHouse.Where(x => x.DebitCode == account && x.DebitDetailCodeFirst == detail1
                                        && (warehouse == string.Empty || x.DebitWarehouse == warehouse) && x.DebitDetailCodeSecond == detail2).Sum(x => x.Amount);
                        var debitQuantity2 = ledgerWareHouse.Where(x => x.DebitCode == account && x.DebitDetailCodeFirst == detail1
                                        && (warehouse == string.Empty || x.DebitWarehouse == warehouse) && x.DebitDetailCodeSecond == detail2).Sum(x => x.Quantity);

                        var creditAmount2Update = ledgerWareHouseUpdate.Where(x => x.CreditCode == account && x.CreditDetailCodeFirst == detail1
                                        && (warehouse == string.Empty || x.CreditWarehouse == warehouse) && x.CreditDetailCodeSecond == detail2).Sum(x => x.Amount);
                        var creditQuantity2Update = ledgerWareHouseUpdate.Where(x => x.CreditCode == account && x.CreditDetailCodeFirst == detail1
                                        && (warehouse == string.Empty || x.CreditWarehouse == warehouse) && x.CreditDetailCodeSecond == detail2).Sum(x => x.Quantity);

                        var debitAmount2Update = ledgerWareHouseUpdate.Where(x => x.DebitCode == account && x.DebitDetailCodeFirst == detail1
                                        && (warehouse == string.Empty || x.DebitWarehouse == warehouse) && x.DebitDetailCodeSecond == detail2).Sum(x => x.Amount);
                        var debitQuantity2Update = ledgerWareHouseUpdate.Where(x => x.DebitCode == account && x.DebitDetailCodeFirst == detail1
                                        && (warehouse == string.Empty || x.DebitWarehouse == warehouse) && x.DebitDetailCodeSecond == detail2).Sum(x => x.Quantity);

                        await UpdateChartOfAccount(detail2, creditAmount2 - creditAmount2Update, creditQuantity2 - creditQuantity2Update,
                            debitAmount2 - debitAmount2Update, debitQuantity2 - debitQuantity2Update, year, account + ":" + detail1, warehouse, isInternal);
                    }
                }
            }
        }
    }

    private async Task UpdateChartOfAccount(string code, double? amountCredit, double? stockQuantityCredit,
        double? amountDebit, double? stockQuantityDebit, int year, string parentRef = "", string wareHouseCode = "", int isInternal = 1)
    {
        if (string.IsNullOrEmpty(code))// || amount == 0
        {
            return;
        }

        ChartOfAccount currentAccount = await _chartOfAccountService.GetAccountByCode(code, year, parentRef, wareHouseCode);

        if (currentAccount == null)
        {
            // Nếu không tìm thấy cũng bỏ qua
            return;
        }

        if (_operator == "-")
        {
            amountCredit = -amountCredit;
            stockQuantityCredit = -stockQuantityCredit;
            amountDebit = -amountDebit;
            stockQuantityDebit = -stockQuantityDebit;
        }
        if (isInternal == 3)
        {
            // Nếu là ngoại tệ
            if (currentAccount.IsForeignCurrency)
            {
                currentAccount.ArisingForeignDebitNB = (currentAccount.ArisingForeignDebitNB ?? 0) + (amountDebit ?? 0);
                currentAccount.ArisingForeignCreditNB = (currentAccount.ArisingForeignCreditNB ?? 0) + (amountCredit ?? 0);
            }
            else // Không phải ngoại tệ
            {
                currentAccount.ArisingDebitNB = (currentAccount.ArisingDebitNB ?? 0) + (amountDebit ?? 0);
                currentAccount.ArisingCreditNB = (currentAccount.ArisingCreditNB ?? 0) + (amountCredit ?? 0);
            }
            currentAccount.ArisingStockQuantityNB = (currentAccount.ArisingStockQuantityNB ?? 0) + (stockQuantityDebit ?? 0) - (stockQuantityCredit ?? 0);

        }
        else if (isInternal == 2)
        {
            // Nếu là ngoại tệ
            if (currentAccount.IsForeignCurrency)
            {
                currentAccount.ArisingForeignDebit = (currentAccount.ArisingForeignDebit ?? 0) + (amountDebit ?? 0);
                currentAccount.ArisingForeignCredit = (currentAccount.ArisingForeignCredit ?? 0) + (amountCredit ?? 0);
            }
            else // Không phải ngoại tệ
            {
                currentAccount.ArisingDebit = (currentAccount.ArisingDebit ?? 0) + (amountDebit ?? 0);
                currentAccount.ArisingCredit = (currentAccount.ArisingCredit ?? 0) + (amountCredit ?? 0);
            }
            currentAccount.ArisingStockQuantity = (currentAccount.ArisingStockQuantity ?? 0) + (stockQuantityDebit ?? 0) - (stockQuantityCredit ?? 0);

        }
        else
        {
            // Nếu là ngoại tệ
            if (currentAccount.IsForeignCurrency)
            {
                currentAccount.ArisingForeignDebit = (currentAccount.ArisingForeignDebit ?? 0) + (amountDebit ?? 0);
                currentAccount.ArisingForeignCredit = (currentAccount.ArisingForeignCredit ?? 0) + (amountCredit ?? 0);
                currentAccount.ArisingForeignDebitNB = (currentAccount.ArisingForeignDebitNB ?? 0) + (amountDebit ?? 0);
                currentAccount.ArisingForeignCreditNB = (currentAccount.ArisingForeignCreditNB ?? 0) + (amountCredit ?? 0);
            }
            else // Không phải ngoại tệ
            {
                currentAccount.ArisingDebit = (currentAccount.ArisingDebit ?? 0) + (amountDebit ?? 0);
                currentAccount.ArisingCredit = (currentAccount.ArisingCredit ?? 0) + (amountCredit ?? 0);
                currentAccount.ArisingDebitNB = (currentAccount.ArisingDebitNB ?? 0) + (amountDebit ?? 0);
                currentAccount.ArisingCreditNB = (currentAccount.ArisingCreditNB ?? 0) + (amountCredit ?? 0);
            }
            currentAccount.ArisingStockQuantity = (currentAccount.ArisingStockQuantity ?? 0) + (stockQuantityDebit ?? 0) - (stockQuantityCredit ?? 0);
            currentAccount.ArisingStockQuantityNB = (currentAccount.ArisingStockQuantityNB ?? 0) + (stockQuantityDebit ?? 0) - (stockQuantityCredit ?? 0);

        }
        // Tính toán số lượng

        _chartOfAccountService.UpdateAccount(currentAccount);
    }

    public async Task<List<LedgerDetailV3Model>> GetLedgerById(long ledgerId, int year)
    {
        var ledgerCheck = await _context.Ledgers.FindAsync(ledgerId);
        if (ledgerCheck is null)
            return new List<LedgerDetailV3Model>();

        List<Ledger> ledgers = await _context.Ledgers.Where(x => x.IsInternal == ledgerCheck.IsInternal && x.OrginalVoucherNumber == ledgerCheck.OrginalVoucherNumber
                            && x.InvoiceCode == ledgerCheck.InvoiceCode
                            && x.InvoiceTaxCode == ledgerCheck.InvoiceTaxCode
                            && x.InvoiceDate == ledgerCheck.InvoiceDate)
                .ToListAsync();

        var listOuts = new List<LedgerDetailV3Model>();
        foreach (var ledger in ledgers)
        {
            var model = _mapper.Map<LedgerDetailV3Model>(ledger);
            model.Debit = await _chartOfAccountV2Service.FindAccount(ledger.DebitCode, string.Empty, year);
            model.Credit = await _chartOfAccountV2Service.FindAccount(ledger.CreditCode, string.Empty, year);
            model.DebitDetailFirst = await _chartOfAccountV2Service.FindAccount(ledger.DebitDetailCodeFirst, ledger.DebitCode, year);
            model.CreditDetailFirst = await _chartOfAccountV2Service.FindAccount(ledger.CreditDetailCodeFirst, ledger.CreditCode, year);
            model.DebitDetailSecond = await _chartOfAccountV2Service.FindAccount(ledger.DebitDetailCodeSecond, ledger.DebitCode + ":" + ledger.DebitDetailCodeFirst, year);
            model.CreditDetailSecond = await _chartOfAccountV2Service.FindAccount(ledger.CreditDetailCodeSecond, ledger.CreditCode + ":" + ledger.CreditDetailCodeFirst, year);
            model.Tab = model.Tab ?? 0;
            listOuts.Add(model);
        }
        return listOuts;
    }

    public async Task UpdateAsync(List<LedgerV3UpdateModel> requests, int year, bool isReverse)
    {

        var data = requests.GroupBy(grp => new { grp.OrginalBookDate.Value.Year, grp.OrginalBookDate.Value.Month }).Select(s =>
                new
                {
                    s.Key.Year,
                    s.Key.Month,
                }
            ).ToList();

        if (data.Any(x => x.Year != year))
        {
            throw new Exception("Năm của ngảy ghi sổ đang khác với năm bạn đang làm việc");
        }

        using var transaction = _context.Database.BeginTransaction();
        var entities = _mapper.Map<List<Ledger>>(requests);

        var entity = entities.FirstOrDefault();
        if (entity is null)
        {
            return;
        }

        int isInternal = entity.IsInternal;
        var ledgerIds = requests.Where(x => x.Id > 0).Select(x => x.Id);

        var ledgerIdUpdate = new List<long>();
        var entityUpdates = entities.Where(x => x.Id > 0).ToList();

        LedgerWareHouse ledgerWareHouse = null;
        var ledgerCurrents = new List<Ledger>();
        if (ledgerIds.Any())
        {
            ledgerCurrents = await _context.GetLedgerNotForYear(isInternal).Where(x => ledgerIds.Contains(x.Id)).ToListAsync();
            ledgerIdUpdate.AddRange(ledgerIds);
            var ledgerId = string.Join(",", ledgerIds);

            ledgerWareHouse = await _context.LedgerWareHouses.FirstOrDefaultAsync(x => x.LedgerIds != null && x.LedgerIds.Contains(ledgerId));
        }

        await UpdateArsingChartOfAccount(entities, isInternal, year, ledgerCurrents);


        var customerId = requests.FirstOrDefault()?.CustomerId;
        var customer = await _context.Customers.FirstOrDefaultAsync(x => x.Id == customerId);
        if (ledgerWareHouse == null)
        {
            ledgerWareHouse = new LedgerWareHouse
            {
                CreateBy = 3,
                IsInternal = isInternal,
                Type = entity.Type,
                CustomerId = customer?.Id,
                Month = entity.Month
            };
        }
        int.TryParse(entity.OrginalVoucherNumber.Split('-').Last(), out int orderNum);

        // Cập nhật thay đổi cho các thực thể tương tự có liên quan
        foreach (var x in entities)
        {
            x.InvoiceTaxCode = entity.InvoiceTaxCode;
            x.InvoiceName = entity.InvoiceName;
            x.InvoiceAddress = entity.InvoiceAddress;
            x.Order = orderNum;
            x.UpdateAt = DateTime.UtcNow;
            await _ledgerUpdateChartOfAccountNameService.UpdateChartOfAccountName(x, year);
            if (isReverse)
            {
                string ledgerString = JsonConvert.SerializeObject(x);
                Ledger item = JsonConvert.DeserializeObject<Ledger>(ledgerString);

                x.CreditCode = item.DebitCode;
                x.CreditCodeName = item.DebitCodeName;
                x.CreditDetailCodeFirst = item.DebitDetailCodeFirst;
                x.CreditDetailCodeFirstName = item.DebitDetailCodeFirstName;
                x.CreditDetailCodeSecond = item.DebitDetailCodeSecond;
                x.CreditDetailCodeSecondName = item.DebitDetailCodeSecondName;
                x.CreditWarehouse = item.DebitWarehouse;
                x.CreditWarehouseName = item.DebitWarehouseName;
                x.DebitCode = item.CreditCode;
                x.DebitCodeName = item.CreditCodeName;
                x.DebitDetailCodeFirst = item.CreditDetailCodeFirst;
                x.DebitDetailCodeFirstName = item.CreditDetailCodeFirstName;
                x.DebitDetailCodeSecond = item.CreditDetailCodeSecond;
                x.DebitDetailCodeSecondName = item.CreditDetailCodeSecondName;
                x.DebitWarehouse = item.CreditWarehouse;
                x.DebitWarehouseName = item.CreditWarehouseName;

            }
        }


        _context.Ledgers.UpdateRange(entityUpdates);

        var ledgerAdds = new List<Ledger>();

        if (entity.IsInternal == 1)
        {
            var entityAdds = entities.Where(x => x.Id == 0).Select(x =>
            {
                x.IsInternal = 2;
                x.CreateAt = DateTime.UtcNow;
                x.Year = year;
                return x;
            }).ToList();

            await _context.Ledgers.AddRangeAsync(entityAdds);
            ledgerAdds.AddRange(entityAdds);

            var entityAddInternals = entityAdds.ConvertAll(x =>
            {
                var y = new Ledger();
                y.CheckAndMap(x);
                y.Id = 0;
                y.UserCreated = x.UserCreated;
                y.CreateAt = x.CreateAt;
                y.IsInternal = 3;
                return y;
            }).ToList();

            await _context.Ledgers.AddRangeAsync(entityAddInternals);
            ledgerAdds.AddRange(entityAddInternals);
        }
        else
        {
            var entityAdds = entities.Where(x => x.Id == 0).Select(x =>
            {
                x.CreateAt = DateTime.UtcNow;
                x.Year = year;
                return x;
            }).ToList();

            await _context.Ledgers.AddRangeAsync(entityAdds);
            ledgerAdds.AddRange(entityAdds);
        }
        // check ccdc
        foreach (var item in entityUpdates)
        {
            await _ledgerFixedAssetService.UpdateAsync(item, year);
        }

        var ledgerWarehouses = entities.Where(x => x.Type == "NK" || entity.Type == "PC" && !string.IsNullOrEmpty(entity.ReferenceVoucherNumber));

        foreach (var ledgerWarehouse in ledgerWarehouses)
        {
            await _goodWarehousesService.Create(entity, year);
        }

        // add ccdc
        var ledgerFixeddAssets = ledgerAdds.Where(X => isInternal == 1 ? X.IsInternal == 2 || X.IsInternal == 1 : X.IsInternal == isInternal);
        foreach (var item in ledgerFixeddAssets)
        {
            // Cập nhật tài sản cố định
            if (entity.DebitCode.Substring(0, 3) == "153")
                await _fixedAssetsService.UpdateFromLedger(entity, year);
            else if (entity.DepreciaMonth > 0)//entity.DebitCode.Substring(0, 3) == "242" &&
                await _fixedAssets242Service.UpdateFromLedger(entity, year);
        }
        ledgerIdUpdate.AddRange(ledgerAdds.Select(X => X.Id));
        ledgerIdUpdate = ledgerIdUpdate.OrderBy(x => x).ToList();

        ledgerWareHouse.UpdateBy = 3;
        ledgerWareHouse.LedgerCount = ledgerIdUpdate.Count;
        ledgerWareHouse.LedgerIds = string.Join(",", ledgerIdUpdate);

        _context.LedgerWareHouses.Update(ledgerWareHouse);
        await _context.SaveChangesAsync();

        await transaction.CommitAsync();
    }
}