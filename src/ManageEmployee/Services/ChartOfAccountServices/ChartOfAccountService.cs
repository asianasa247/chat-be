using AutoMapper;
using Hangfire;
using ManageEmployee.Queues;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.DataTransferObject.ChartOfAccountModels;
using ManageEmployee.Services.Interfaces.Goods;

namespace ManageEmployee.Services.ChartOfAccountServices;

public class ChartOfAccountService : IChartOfAccountService
{
    private readonly ApplicationDbContext _context;
    private readonly IChartOfAccountCaculatorQueue _chartOfAccountCaculatorQueue;
    private readonly IMapper _mapper;
    private readonly ICharOfAccountSyncService _charOfAccountSyncService;
    private readonly IChartOfAccountCalculateBalancer _chartOfAccountCalculateBalancer;
    private readonly IGoodSynchronizer _goodSynchronizer;

    public ChartOfAccountService(
        ApplicationDbContext context,
        IChartOfAccountCaculatorQueue chartOfAccountCaculatorQueue,
        IMapper mapper,
        ICharOfAccountSyncService charOfAccountSyncService,
        IChartOfAccountCalculateBalancer chartOfAccountCalculateBalancer
, IGoodSynchronizer goodSynchronizer)
    {
        _context = context;
        _chartOfAccountCaculatorQueue = chartOfAccountCaculatorQueue;
        _mapper = mapper;
        _charOfAccountSyncService = charOfAccountSyncService;
        _chartOfAccountCalculateBalancer = chartOfAccountCalculateBalancer;
        _goodSynchronizer = goodSynchronizer;
    }

    public async Task<string> Create(ChartOfAccount entity, int year)
    {
        try
        {
            entity.HasChild = false;
            entity.HasDetails = false;
            entity.DisplayDelete = true;
            entity.DisplayInsert = true;
            entity.Year = year;

            using var transaction = await _context.Database.BeginTransactionAsync();

            int lengthCode = entity.Code.Length;
            string parentRef = entity.Code.Substring(0, lengthCode - 1);

            var accountGroup = await _context.GetChartOfAccountGroupLink(year).FirstOrDefaultAsync(x => x.CodeChartOfAccount == parentRef);
            if (accountGroup != null)
                return ChartOfAccountResultErrorConstants.ACCOUNT_GROUP_LINK;
            var account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == entity.Code);
            if (account != null)
                return ChartOfAccountResultErrorConstants.ACCOUNT_EXIST;

            if (lengthCode == 3)
            {
                entity.Type = 1;
                _context.ChartOfAccounts.Add(entity);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return string.Empty;
            }
            entity.ParentRef = parentRef;

            var ledger = await _context.GetLedger(year).FirstOrDefaultAsync(x => string.IsNullOrEmpty(entity.ParentRef) || x.CreditCode == entity.ParentRef || x.DebitCode == entity.ParentRef);
            if (ledger != null)
            {
                return ChartOfAccountResultErrorConstants.ACCOUNT_LEDGER;
            }

            var accountParent = await _context.GetChartOfAccount(year).AsNoTracking().FirstOrDefaultAsync(x => x.Code == entity.ParentRef);
            if (accountParent == null)
                return ChartOfAccountResultErrorConstants.ACCOUNT_PARENT_NOT_EXIST;
            accountParent.DisplayInsert = false;
            accountParent.DisplayDelete = false;

            if (lengthCode == 4)
                entity.Type = 2;
            if (lengthCode == 5)
                entity.Type = 3;
            if (lengthCode == 6)
                entity.Type = 4;

            if (accountParent.HasDetails)
            {
                // remove detail if parent has detail
                var details = await _context.GetChartOfAccount(year).Where(x => x.Type > 4 &&
                x.ParentRef.StartsWith(accountParent.Code)).ToListAsync();
                if (details.Any())
                {
                    details = details.ConvertAll(x => { x.ParentRef = x.ParentRef.Replace(accountParent.Code, entity.Code); return x; });
                }
                _context.ChartOfAccounts.UpdateRange(details);

                entity.HasChild = false;
                entity.HasDetails = true;
                entity.DisplayInsert = true;
                entity.DisplayDelete = false;
            }
            accountParent.HasChild = true;
            accountParent.HasDetails = false;

            _context.ChartOfAccounts.Update(accountParent);
            _context.ChartOfAccounts.Add(entity);
            await _context.SaveChangesAsync();
            await _chartOfAccountCalculateBalancer.CalculateBalance(entity, year, accountParent);
            await transaction.CommitAsync();
            return string.Empty;
        }
        catch (Exception ex)
        {
            return ex.Message.ToString();
        }
    }

    public async Task<ObjectReturn> CheckAccountTest(string code, int year)
    {
        var account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == code);
        if (account != null)
            return new ObjectReturn
            {
                message = ChartOfAccountResultErrorConstants.ACCOUNT_EXIST,
                status = 200,
                code = "ACCOUNT_EXIST",
            };

        var accountTest = await _context.ChartOfAcountTests.FirstOrDefaultAsync(x => x.Code == code);
        if (accountTest == null)
            return new ObjectReturn
            {
                message = ChartOfAccountResultErrorConstants.NOT_ACCOUNT_TEST,
                status = 200,
            };
        return new ObjectReturn
        {
            message = "",
            status = 200,
            code = "ACCOUNT_TEST",
            data = accountTest
        };
    }


    public async Task<string> CreateDetail(ChartOfAccount entity, int year)
    {
        var warehouses = await _context.Warehouses.Where(x => !x.IsDelete).ToListAsync();

        if (!string.IsNullOrEmpty(entity.WarehouseCode))
        {
            entity.WarehouseName = warehouses.Find(x => x.Code == entity.WarehouseCode)?.Name;
        }
        entity.HasChild = false;
        entity.HasDetails = false;
        entity.DisplayDelete = true;
        entity.DisplayInsert = true;
        entity.Year = year;

        entity.Id = 0;
        using var transaction = await _context.Database.BeginTransactionAsync();
        var accountExit = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.WarehouseCode == entity.WarehouseCode && x.Code == entity.Code && x.ParentRef == entity.ParentRef);
        if (accountExit != null)
            return ChartOfAccountResultErrorConstants.ACCOUNT_EXIST;

        string parentCode = entity.ParentRef;
        ChartOfAccount accountParent;
        string grandParentRef = "";
        Ledger ledger;

        if (entity.ParentRef.Contains(":"))
        {
            string[] segments = entity.ParentRef.Split(':', StringSplitOptions.RemoveEmptyEntries);
            parentCode = segments[1];
            grandParentRef = segments[0];

            accountParent = await _context.GetChartOfAccount(year).SingleOrDefaultAsync(x =>
                x.Code == parentCode && x.ParentRef == grandParentRef && x.WarehouseCode == entity.WarehouseCode);

            ledger = await _context.GetLedger(year).FirstOrDefaultAsync(x => (x.DebitDetailCodeSecond == entity.Code || x.CreditDetailCodeSecond == entity.Code)
                    && (x.CreditWarehouse == entity.WarehouseCode || x.DebitWarehouse == entity.WarehouseCode || string.IsNullOrEmpty(entity.WarehouseCode))
                     && (x.CreditDetailCodeFirst == parentCode || x.DebitDetailCodeFirst == parentCode)
                     && (x.DebitCode == grandParentRef || x.CreditCode == grandParentRef));
        }
        else
        {
            accountParent = await _context.GetChartOfAccount(year).SingleOrDefaultAsync(x => x.Type < 5 &&
            x.Code == entity.ParentRef);
            ledger = await _context.GetLedger(year).FirstOrDefaultAsync(x => (x.DebitCode == parentCode || x.CreditCode == parentCode)
                     && (x.CreditDetailCodeFirst == entity.Code || x.DebitDetailCodeFirst == entity.Code));
        }

        if (accountParent is null)
            throw new ErrorException("ParentRef sai data");

        if (ledger == null)
        {
            accountParent.HasChild = false;
            accountParent.HasDetails = true;
            accountParent.DisplayDelete = false;
            _context.ChartOfAccounts.Update(accountParent);
        }
        if (entity.ParentRef.Contains(":"))
        {
            var detail1 = _context.GetChartOfAccount(year).Where(x => x.ParentRef == parentCode).ToList();
            foreach (var account in detail1)
            {
                account.HasDetails = true;
                account.HasChild = false;
                account.DisplayDelete = false;
                _context.ChartOfAccounts.Update(account);
                string ParentRef = parentCode + ":" + account.Code;
                if (entity.ParentRef == ParentRef)
                    continue;
                var account_new = new ChartOfAccount();
                account_new.Code = entity.Code;
                account_new.Name = entity.Name;
                account_new.ParentRef = ParentRef;
                account_new.Duration = account.Duration;
                account_new.AccGroup = account.AccGroup;
                account_new.Classification = account.Classification;
                account_new.Protected = account.Protected;
                account_new.WarehouseCode = account.WarehouseCode;
                account_new.WarehouseName = account.WarehouseName;
                account_new.Type = account.Type;
                account_new.StockUnit = entity.StockUnit;
                account_new.Year = year;

                await _context.ChartOfAccounts.AddAsync(account_new);
            }
        }

        var accountChanges = await _charOfAccountSyncService.SyncAccountGroupAsync(entity, grandParentRef, parentCode, warehouses, accountParent, year);

        await _context.ChartOfAccounts.AddRangeAsync(accountChanges.accountAdds);
        _context.ChartOfAccounts.UpdateRange(accountChanges.accountUpdates);

        await _context.ChartOfAccounts.AddAsync(entity);
        await _context.SaveChangesAsync();
        await _chartOfAccountCalculateBalancer.CalculateBalance(entity, year, accountParent);
        await transaction.CommitAsync();
        return string.Empty;
    }

    public async Task<string> DeleteDetail(long id, int year)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();
            var account = await _context.ChartOfAccounts.FindAsync(id);
            _context.ChartOfAccounts.Remove(account);
            await _context.SaveChangesAsync();
            await _chartOfAccountCalculateBalancer.CalculateBalance(account, year);
            _context.Database.CommitTransaction();
            return string.Empty;
        }
        catch (Exception ex)
        {
            _context.Database.RollbackTransaction();
            return ex.Message.ToString();
        }
    }

    public async Task<List<ChartOfAccountModel>> GetAllAccounts(PagingRequestModel param, int year)
    {
        var query = GetAccountQuery(year);
        query = query.Where(x => x.Type <= 4);

        if (!string.IsNullOrEmpty(param.SearchText))
        {
            query = query
                .Where(x => x.Code.Contains(param.SearchText) || x.Name.Contains(param.SearchText) ||
                             x.OpeningCredit.ToString().Contains(param.SearchText) ||
                             x.OpeningDebit.ToString().Contains(param.SearchText))
                .Distinct()
                .OrderBy(x => x.Code);
        }

        if (param.Page > 0)
            return await query.Skip((param.Page - 1) * param.PageSize)
                .Take(param.PageSize).ToListAsync();

        return await query
            .Distinct()
            .OrderBy(x => x.Code)
            .ToListAsync();
    }

    public async Task<List<ChartOfAccountModel>> GetAllDetails(int currentPage, int pageSize, string parentCode,
        string warehouseCode, string searchCode, int year, int id = 0, int isInternal = 0)
    {
        try
        {
            IQueryable<ChartOfAccountModel> query;
            if (!parentCode.Contains(":"))
                query = from coa in _context.GetChartOfAccount(year)
                        where coa.Type == 5
                        && coa.ParentRef == parentCode
                        && (isInternal == 0 || coa.IsInternal == isInternal || coa.IsInternal == 1 || coa.IsInternal == 0)
                        && (string.IsNullOrEmpty(warehouseCode) || coa.WarehouseCode == warehouseCode)
                        select new ChartOfAccountModel()
                        {
                            Code = coa.Code,
                            Duration = coa.Duration,
                            Classification = coa.Classification,
                            Protected = coa.Protected,
                            Id = coa.Id,
                            Name = coa.Name,
                            Type = coa.Type,
                            ArisingCredit = coa.ArisingCredit,
                            ArisingDebit = coa.ArisingDebit,
                            DisplayDelete = coa.DisplayDelete,
                            StockUnitPrice = coa.StockUnitPrice,
                            HasDetails = coa.HasDetails,
                            HasChild = coa.HasChild,
                            ParentRef = coa.ParentRef,
                            IsForeignCurrency = coa.IsForeignCurrency,
                            WarehouseCode = coa.WarehouseCode,
                            WarehouseName = coa.WarehouseName,
                            AccGroup = coa.AccGroup,
                            StockUnit = coa.StockUnit,

                            OpeningDebit = coa.OpeningDebit,
                            OpeningCredit = coa.OpeningCredit,
                            OpeningForeignCredit = coa.OpeningForeignCredit,
                            OpeningForeignDebit = coa.OpeningForeignDebit,

                            OpeningStockQuantity = coa.OpeningStockQuantity,
                            OpeningStockQuantityNB = coa.OpeningStockQuantityNB,
                            StockUnitPriceNB = coa.StockUnitPriceNB,
                            OpeningDebitNB = coa.OpeningDebitNB,
                            OpeningCreditNB = coa.OpeningCreditNB,
                            OpeningForeignCreditNB = coa.OpeningForeignCreditNB,
                            OpeningForeignDebitNB = coa.OpeningForeignDebitNB,
                            ClosingDebit = coa.OpeningDebit - coa.ArisingDebit,
                            ClosingCredit = coa.OpeningCredit - coa.ArisingCredit,
                            ClosingStockQuantity = coa.OpeningStockQuantity - coa.ArisingStockQuantity,
                            ClosingAmount = coa.OpeningDebit - coa.OpeningCredit + coa.ArisingDebit - coa.ArisingCredit,
                            IsInternal = coa.IsInternal
                        };
            else
            {
                query = from coa in _context.GetChartOfAccount(year)
                        where coa.Type == 6
                        && coa.ParentRef == parentCode
                        && (isInternal == 0 || coa.IsInternal == isInternal || coa.IsInternal == 1 || coa.IsInternal == 0)

                        select new ChartOfAccountModel()
                        {
                            Duration = coa.Duration,
                            Classification = coa.Classification,
                            Protected = coa.Protected,
                            AccGroup = coa.AccGroup,
                            Code = coa.Code,
                            Name = coa.Name,
                            StockUnitPrice = coa.StockUnitPrice,
                            HasDetails = coa.HasDetails,
                            OpeningStockQuantityNB = coa.OpeningStockQuantityNB,
                            StockUnitPriceNB = coa.StockUnitPriceNB,
                            OpeningDebitNB = coa.OpeningDebitNB,
                            OpeningCreditNB = coa.OpeningCreditNB,
                            OpeningForeignCreditNB = coa.OpeningForeignCreditNB,
                            OpeningForeignDebitNB = coa.OpeningForeignDebitNB,
                            ParentRef = coa.ParentRef,
                            IsForeignCurrency = coa.IsForeignCurrency,
                            WarehouseCode = coa.WarehouseCode,
                            WarehouseName = coa.WarehouseName,
                            Id = coa.Id,
                            StockUnit = coa.StockUnit,

                            ClosingDebit = coa.OpeningDebit - coa.ArisingDebit,
                            ClosingCredit = coa.OpeningCredit - coa.ArisingCredit,
                            ClosingStockQuantity = coa.OpeningStockQuantity - coa.ArisingStockQuantity,
                            ClosingAmount = coa.OpeningDebit - coa.OpeningCredit + coa.ArisingDebit - coa.ArisingCredit,

                            OpeningDebit = coa.OpeningDebit,
                            OpeningCredit = coa.OpeningCredit,
                            OpeningForeignCredit = coa.OpeningForeignCredit,
                            OpeningForeignDebit = coa.OpeningForeignDebit,
                            OpeningStockQuantity = coa.OpeningStockQuantity,
                            DisplayDelete = coa.DisplayDelete,
                            IsInternal = coa.IsInternal
                        };
            }

            var defQuery = query;

            if (query.Count() <= 0)
            {
                query = defQuery;
            }
            if (id != 0)
            {
                return await query.Where(w => w.Id == id).Skip(0).Take(1).ToListAsync();
            }

            if (!string.IsNullOrEmpty(searchCode))
            {
                query = query.Where(x => x.Name.ToLower().Contains(searchCode.ToLower()) || x.Code.ToLower().Contains(searchCode.ToLower()));
            }

            if (!string.IsNullOrEmpty(warehouseCode))
            {
                query = query.Where(x => x.WarehouseCode == warehouseCode);
            }

            if (currentPage == 0)
                return await query
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return await query
                .OrderByDescending(x => x.Id)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new ErrorException(ex.Message.ToString());
        }
    }

    public async Task<int> Count(int year, Expression<Func<ChartOfAccount, bool>> @where = null)
    {
        if (@where == null)
            return await _context.GetChartOfAccount(year).CountAsync();
        return await _context.GetChartOfAccount(year).Where(@where).CountAsync();
    }

    public async Task<IEnumerable<ChartOfAccountModel>> GetAllByDisplayInsert(PagingRequestModel param, int year)
    {
        var accounts = await GetAllAccounts(param, year);
        return accounts.Where(x => x.DisplayInsert).ToList();
    }

    public async Task<List<ChartOfAccountModel>> ExportGetAllArisingAccounts(int year)
    {
        var query = from coa in _context.GetChartOfAccount(year)
                    where coa.Type > 0
                    select new ChartOfAccountModel()
                    {
                        Code = coa.Code,
                        Name = coa.Name,
                        ParentRef = coa.ParentRef,
                        WarehouseCode = coa.WarehouseCode,
                        StockUnit = coa.StockUnit,
                        OpeningStockQuantityNB = coa.OpeningStockQuantityNB,
                        StockUnitPriceNB = coa.StockUnitPriceNB,
                        OpeningDebitNB = coa.OpeningDebitNB,
                        OpeningCreditNB = coa.OpeningCreditNB,
                        OpeningForeignCreditNB = coa.OpeningForeignCreditNB,
                        OpeningForeignDebitNB = coa.OpeningForeignDebitNB,
                        AccGroup = coa.AccGroup,
                        Classification = coa.Classification,
                        StockUnitPrice = coa.StockUnitPrice,
                        OpeningForeignCredit = (coa.OpeningForeignDebit ?? 0) + (coa.ArisingForeignDebit ?? 0) - (coa.OpeningForeignCredit ?? 0) - (coa.ArisingForeignCredit ?? 0),
                        OpeningCredit = (coa.OpeningDebit ?? 0) + (coa.ArisingDebit ?? 0) - (coa.OpeningCredit ?? 0) - (coa.ArisingCredit ?? 0),
                    };

        return await query
            .OrderBy(x => x.ParentRef)
            .ToListAsync();
    }

    public async Task<string> ImportFromExcelTaiKhoanArising(List<ChartOfAccount> data, int year)
    {
        try
        {
            var listTaiKhoan = await _context.GetChartOfAccount(year).ToListAsync();

            foreach (var itrm in data)
            {
                var itemFind = listTaiKhoan.Find(x => x.Code == itrm.Code && x.ParentRef == itrm.ParentRef && x.WarehouseCode == itrm.WarehouseCode);
                if (itemFind == null)
                    itemFind = new ChartOfAccount();
                if (itemFind.Id > 0)
                {
                    itemFind.WarehouseCode = itrm.WarehouseCode;
                    itemFind.OpeningStockQuantity = itrm.OpeningStockQuantity;
                    itemFind.StockUnitPrice = itrm.StockUnitPrice;
                    itemFind.OpeningDebit = itrm.OpeningDebit;
                    itemFind.OpeningCredit = itrm.OpeningCredit;
                    itemFind.OpeningForeignDebit = itrm.OpeningForeignDebit;
                    itemFind.OpeningForeignCredit = itrm.OpeningForeignCredit;
                    itemFind.AccGroup = itrm.AccGroup;
                    itemFind.Classification = itrm.Classification;
                    _context.ChartOfAccounts.Update(itemFind);
                }
            }
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        return string.Empty;
    }

    public async Task<List<LookupValue>> GetLookupValues(string scope)
    {
        var values = await _context.LookupValues.Where(x => x.Scope == scope).ToListAsync();
        return values;
    }

    public async Task<List<ChartOfAccountModel>> ExportAccountChiTiet1(string code, bool isExportAll, string warehouseCode, int year)
    {
        var query = _context.GetChartOfAccount(year).Where(x => (x.ParentRef.Contains(code) || x.ParentRef == code)
                                        && (string.IsNullOrEmpty(warehouseCode) || x.WarehouseCode == warehouseCode));

        if (!code.Contains(":"))
        {
            query = query.Where(x => x.Type == 5 || isExportAll);
        }
        else
        {
            query = query.Where(x => x.Type == 6);
        }
        var queryModel = query.Select(coa => new ChartOfAccountModel()
        {
            Code = coa.Code,
            Name = coa.Name,
            StockUnit = coa.StockUnit,
            OpeningStockQuantityNB = coa.OpeningStockQuantityNB,
            StockUnitPriceNB = coa.StockUnitPriceNB,
            OpeningDebitNB = coa.OpeningDebitNB,
            OpeningCreditNB = coa.OpeningCreditNB,
            OpeningForeignCreditNB = coa.OpeningForeignCreditNB,
            OpeningForeignDebitNB = coa.OpeningForeignDebitNB,
            AccGroup = coa.AccGroup,
            Classification = coa.Classification,
            OpeningStockQuantity = coa.OpeningStockQuantity,
            StockUnitPrice = coa.StockUnitPrice,
            OpeningDebit = coa.OpeningDebit,
            OpeningCredit = coa.OpeningCredit,
            OpeningForeignCredit = coa.OpeningForeignCredit,
            OpeningForeignDebit = coa.OpeningForeignDebit,
            ParentRef = coa.ParentRef,
            Type = coa.Type,
            WarehouseCode = coa.WarehouseCode,
            WarehouseName = coa.WarehouseName,
            Id = coa.Id
        });
        return await queryModel
            .OrderBy(x => x.Code)
            .ToListAsync();
    }

    public List<ChartOfAccountModel> ExportGetAllAccounts(int year)
    {
        var query = from coa in _context.GetChartOfAccount(year)
                    where coa.Type <= 4 && coa.Type > 0
                    select new ChartOfAccountModel()
                    {
                        Code = coa.Code,
                        Name = coa.Name,
                        StockUnit = coa.StockUnit,
                        OpeningStockQuantityNB = coa.OpeningStockQuantityNB,
                        StockUnitPriceNB = coa.StockUnitPriceNB,
                        OpeningDebitNB = coa.OpeningDebitNB,
                        OpeningCreditNB = coa.OpeningCreditNB,
                        OpeningForeignCreditNB = coa.OpeningForeignCreditNB,
                        OpeningForeignDebitNB = coa.OpeningForeignDebitNB,
                        AccGroup = coa.AccGroup,
                        Classification = coa.Classification,

                        OpeningStockQuantity = coa.OpeningStockQuantity,
                        StockUnitPrice = coa.StockUnitPrice,
                        OpeningDebit = coa.OpeningDebit,
                        OpeningCredit = coa.OpeningCredit,
                        OpeningForeignCredit = coa.OpeningForeignCredit,
                        OpeningForeignDebit = coa.OpeningForeignDebit,
                    };

        return query
            .Distinct()
            .OrderBy(x => x.Code)
            .ToList();
    }

    public List<Warehouse> GetListWarehouse()
    {
        return _context.Warehouses
            .Where(x => !x.IsDelete)
                .OrderBy(x => x.Name).ToList();
    }

    public async Task<string> ImportFromExcel(List<ChartOfAccount> data, int year)
    {
        using (var trans = _context.Database.BeginTransaction())
        {
            try
            {
                var listTaiKhoan = await _context.GetChartOfAccount(year).AsNoTracking().ToListAsync();
                List<string> codeParents = new List<string>();

                foreach (var itrm in data)
                {
                    if (itrm.Code.Length > 8)
                    {
                        await _context.Database.RollbackTransactionAsync();
                        return "Mã " + itrm.Code + " dài hơn 8 kí tự";
                    }
                    var itemFind = listTaiKhoan.Find(x => x.Code == itrm.Code);
                    if (itemFind == null)
                    {
                        itemFind = new ChartOfAccount();
                        itemFind.Year = year;
                    }

                    if (itemFind.HasDetails)
                        continue;

                    itemFind.Code = itrm.Code;
                    itemFind.Duration = string.IsNullOrEmpty(itrm.Duration) ? "N" : itrm.Duration;
                    itemFind.WarehouseCode = itrm.WarehouseCode;
                    DefineAccountTypeAndParentRef(itemFind);
                    itemFind.Name = itrm.Name;
                    if (itrm.IsInternal == 3)
                    {
                        itemFind.OpeningDebitNB = itrm.OpeningDebitNB;
                        itemFind.OpeningCreditNB = itrm.OpeningCreditNB;
                        itemFind.OpeningForeignDebitNB = itrm.OpeningForeignDebitNB;
                        itemFind.OpeningForeignCreditNB = itrm.OpeningForeignCreditNB;
                    }
                    else
                    {
                        itemFind.OpeningDebit = itrm.OpeningDebit;
                        itemFind.OpeningCredit = itrm.OpeningCredit;
                        itemFind.OpeningForeignDebit = itrm.OpeningForeignDebit;
                        itemFind.OpeningForeignCredit = itrm.OpeningForeignCredit;
                    }

                    itemFind.AccGroup = itrm.AccGroup;
                    itemFind.Classification = itrm.Classification;
                    if (!string.IsNullOrEmpty(itemFind.ParentRef))
                    {
                        string codeParent = codeParents.Find(x => x == itemFind.ParentRef);
                        if (string.IsNullOrEmpty(codeParent))
                            codeParents.Add(itemFind.ParentRef);
                    }
                    if (itemFind.Id == 0)
                        listTaiKhoan.Add(itemFind);
                }
                await UpdateArisingAccountImport(listTaiKhoan);
                await _context.SaveChangesAsync();

                await _context.Database.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _context.Database.RollbackTransactionAsync();
                return ex.Message;
            }
        }
        return string.Empty;
    }

    private async Task<string> UpdateArisingAccountImport(List<ChartOfAccount> listAccount)
    {
        try
        {
            var accountParents = listAccount.Where(x => string.IsNullOrEmpty(x.ParentRef));
            foreach (var account in accountParents)
            {
                DeQuyUpdateChartOfAccount(listAccount, account);
            }
            await _context.SaveChangesAsync();
            return string.Empty;
        }
        catch (Exception e)
        {
            return e.Message;
        }
    }

    private void DefineAccountTypeAndParentRef(ChartOfAccount entity)
    {
        switch (entity.Code.Length)
        {
            case 3:
                entity.Type = 1;
                break;

            case 4:
                entity.Type = 2;
                entity.ParentRef = entity.Code.Substring(0, 3);
                break;

            case 5:
                entity.Type = 3;
                entity.ParentRef = entity.Code.Substring(0, 4);
                break;

            case 6:
                entity.Type = 4;
                entity.ParentRef = entity.Code.Substring(0, 5);
                break;
        }
    }

    public async Task<string> ImportFromExcelCT1(List<ChartOfAccountImportModel> data, string codeParent, int year)
    {
        List<ChartOfAccountImportCheckModel> itemChecks = new List<ChartOfAccountImportCheckModel>();
        foreach (var itrm in data)
        {
            var itemCheck = itemChecks.Find(x => x.Code == itrm.Code && x.ParentCode == itrm.ParentRef && x.WarehouseCode == itrm.WarehouseCode);
            if (itemCheck == null)
                itemChecks.Add(new ChartOfAccountImportCheckModel
                {
                    Code = itrm.Code,
                    ParentCode = itrm.ParentRef,
                    WarehouseCode = itrm.WarehouseCode
                });
            else
            {
                throw new Exception("File excel trùng mã " + itemCheck.Code);
            }
        }
        var listAccount = await _context.GetChartOfAccount(year).ToListAsync();
        var warehouses = await _context.Warehouses.Where(x => !x.IsDelete).ToListAsync();
        var entities = new List<ChartOfAccount>();
        var accountChanges = new List<ChartOfAccount>();

        var codeParent_begin = codeParent;

        foreach (var item in data)
        {
            string codeParentExcel = codeParent_begin;
            var itemParent = listAccount.Find(x => x.Code == codeParent && (string.IsNullOrEmpty(item.WarehouseCode) || x.WarehouseCode == item.WarehouseCode || x.Type < 5));
            if (!string.IsNullOrEmpty(item.ParentRef))
            {
                codeParentExcel = codeParent + ":" + item.ParentRef;
                itemParent = listAccount.Find(x => x.Code == item.ParentRef && x.ParentRef == codeParent
                                && (string.IsNullOrEmpty(item.WarehouseCode) || x.WarehouseCode == item.WarehouseCode || x.Type < 5));
            }
            if (codeParent.Contains(":"))
            {
                var codeParents = codeParent.Split(":");
                itemParent = listAccount.Find(x => x.Code == codeParents[1] && x.ParentRef == codeParents[0]
                                && (string.IsNullOrEmpty(item.WarehouseCode) || x.WarehouseCode == item.WarehouseCode || x.Type < 5));
            }


            if (itemParent == null)
            {
                throw new ErrorException("Không tìm thấy mã cha của " + item.Code);
            }

            var itemFind = listAccount.Find(x => x.Code == item.Code && x.ParentRef == codeParentExcel && (string.IsNullOrEmpty(item.WarehouseCode) || x.WarehouseCode == item.WarehouseCode));
            if (itemFind == null)
            {
                itemFind = new ChartOfAccount();
                itemFind.Year = year;
            }
            else if (itemFind.HasChild)
                continue;

            if (!string.IsNullOrEmpty(item.WarehouseCode))
            {
                item.WarehouseName = warehouses.Find(x => x.Code == item.WarehouseCode)?.Name;
            }

            item.DisplayDelete = true;
            if (codeParentExcel.Contains(":"))
            {
                item.Type = 6;
                if (string.IsNullOrEmpty(item.WarehouseCode))
                {
                    item.WarehouseCode = itemParent.WarehouseCode;
                }
            }
            else
            {
                item.Type = 5;
                item.DisplayInsert = true;
            }
            item.ParentRef = codeParentExcel;
            if (item.TypeInternal != 3)
            {
                itemParent.StockUnitPrice = 0;
                itemParent.OpeningStockQuantity = 0;
            }
            else
            {
                itemParent.StockUnitPriceNB = 0;
                itemParent.OpeningStockQuantityNB = 0;
            }
            if (itemFind.Id > 0)
            {
                itemFind.Duration = string.IsNullOrEmpty(itemParent.Duration) ? "N" : itemParent.Duration;
                itemFind.WarehouseCode = item.WarehouseCode;
                if (item.TypeInternal != 3)
                {
                    itemFind.OpeningStockQuantity = item.OpeningStockQuantity;
                    itemFind.StockUnitPrice = item.StockUnitPrice;
                    itemFind.OpeningDebit = item.OpeningDebit;
                    itemFind.OpeningCredit = item.OpeningCredit;
                    itemFind.OpeningForeignDebit = item.OpeningForeignDebit;
                    itemFind.OpeningForeignCredit = item.OpeningForeignCredit;
                    itemFind.OpeningForeignCredit = item.OpeningForeignCredit;
                    itemParent.OpeningStockQuantity = 0;
                }
                else
                {
                    itemFind.OpeningStockQuantityNB = item.OpeningStockQuantityNB;
                    itemFind.StockUnitPriceNB = item.StockUnitPriceNB;
                    itemFind.OpeningDebitNB = item.OpeningDebitNB;
                    itemFind.OpeningCreditNB = item.OpeningCreditNB;
                    itemFind.OpeningForeignDebitNB = item.OpeningForeignDebitNB;
                    itemFind.OpeningForeignCreditNB = item.OpeningForeignCreditNB;
                    itemParent.StockUnitPriceNB = 0;
                    itemParent.OpeningStockQuantityNB = 0;
                }
                itemFind.AccGroup = item.AccGroup;
                itemFind.Classification = item.Classification;
                itemFind.IsInternal = item.IsInternal;
                itemFind.StockUnit = item.StockUnit;
                accountChanges.Add(itemFind);
            }
            else
            {
                item.Duration = string.IsNullOrEmpty(itemParent.Duration) ? "N" : itemParent.Duration;
                item.Year = year;
                var accountMap = _mapper.Map<ChartOfAccount>(item);
                listAccount.Add(accountMap);
                entities.Add(accountMap);
            }

            itemParent.HasDetails = true;
            itemParent.HasChild = true;
            itemParent.DisplayDelete = false;

        }

        var accountSyncs = await _charOfAccountSyncService.SyncListAccountGroupAsync(entities.OrderBy(x => x.Type).ToList(), codeParent, warehouses, listAccount, year);
        listAccount.AddRange(accountSyncs.accountAdds);

        // update goods
        await SyncGoodAsync(data, listAccount);
        accountChanges.AddRange(accountSyncs.accountUpdates);
        UpdateAccountImport(listAccount, accountSyncs.accountUpdates);
        await _context.SaveChangesAsync();
        return string.Empty;
    }

    private async Task SyncGoodAsync(List<ChartOfAccountImportModel> data, List<ChartOfAccount> accounts)
    {
        var accountToSyncGoods = new List<GoodChartOfAccountUpdateModel>();
        var accountChecks = accounts.Where(x => (x.Classification == 2 || x.Classification == 3));
        foreach(var accountToSyncGood in accountToSyncGoods)
        {
            var itemOut = new GoodChartOfAccountUpdateModel
            {
                Code = accountToSyncGood.Code,
                HasDetails = accountToSyncGood.HasDetails,
                OpeningStockQuantityNB = accountToSyncGood.OpeningStockQuantityNB,
                Name = accountToSyncGood.Name,
                ParentRef = accountToSyncGood.ParentRef,
                StockUnit = accountToSyncGood.StockUnit,
                Type = accountToSyncGood.Type,
                WarehouseCode = accountToSyncGood.WarehouseCode,
                WarehouseName = accountToSyncGood.WarehouseName,
            };

            var accountCheck = data.FirstOrDefault(x => x.Code == accountToSyncGood.Code 
                                                        && x.ParentRef == accountToSyncGood.ParentRef 
                                                        && (string.IsNullOrEmpty(x.WarehouseCode) || x.WarehouseCode == accountToSyncGood.WarehouseCode));
            if(accountCheck != null)
            {
                itemOut.UnitPrice = accountCheck.UnitPrice;
            }
            accountToSyncGoods.Add(itemOut);
        }
        
        await _goodSynchronizer.SetGoodFromAccountAsync(accountToSyncGoods);

    }
    private void UpdateAccountImport(List<ChartOfAccount> listAccount, List<ChartOfAccount> accountUpdates)
    {
        List<ChartOfAccount> accountAdds = listAccount.Where(x => x.Id == 0).ToList();
        var parentRefs = accountUpdates.Concat(accountAdds).OrderByDescending(x => x.Type)
                             .GroupBy(x => x.ParentRef)
                             .Select(x => x.Key).ToList();
        var accountUpdateIds = accountUpdates.Select(x => x.Id).ToList();
        foreach (var parentRef in parentRefs)
        {
            DeQuyUpdateAccountImport(listAccount, parentRef, accountUpdateIds);
        }

        _context.ChartOfAccounts.AddRange(accountAdds);
        _context.ChartOfAccounts.UpdateRange(accountUpdates);

    }

    private void DeQuyUpdateAccountImport(List<ChartOfAccount> listAccount, string parentRef, List<long> accountUpdateIds)
    {
        if (string.IsNullOrEmpty(parentRef))
            return;

        var codeParent = parentRef;
        var parentRefParent = "";
        if (parentRef.Contains(":"))
        {
            codeParent = parentRef.Split(":")[1];
            parentRefParent = parentRef.Split(":")[0];
        }
        var account = listAccount.Find(x => x.Code == codeParent && (string.IsNullOrEmpty(parentRefParent) || x.ParentRef == parentRefParent));
        if (account is null)
            return;

        var accountChidrens = listAccount.Where(x => x.ParentRef == parentRef
        && (x.WarehouseCode == account.WarehouseCode || string.IsNullOrEmpty(x.WarehouseCode))).ToList();

        if (accountChidrens.Count > 0)
        {
            account.OpeningDebit = accountChidrens.Sum(x => x.OpeningDebit);
            account.OpeningCredit = accountChidrens.Sum(x => x.OpeningCredit);
            account.OpeningForeignDebit = accountChidrens.Sum(x => x.OpeningForeignDebit);
            account.OpeningForeignCredit = accountChidrens.Sum(x => x.OpeningForeignCredit);

            account.OpeningDebitNB = accountChidrens.Sum(x => x.OpeningDebitNB);
            account.OpeningCreditNB = accountChidrens.Sum(x => x.OpeningCreditNB);
            account.OpeningForeignDebitNB = accountChidrens.Sum(x => x.OpeningForeignDebitNB);
            account.OpeningForeignCreditNB = accountChidrens.Sum(x => x.OpeningForeignCreditNB);

            
        }
        if (account.Id > 0 && !accountUpdateIds.Contains(account.Id))
        {
            _context.ChartOfAccounts.Update(account);
        }

        DeQuyUpdateAccountImport(listAccount, account.ParentRef, accountUpdateIds);
    }


    public async Task<ChartOfAccount> GetAccountByCode(string accountCode, int year, string parentRef = "", string wareHouseCode = "")
    {
        if (!string.IsNullOrEmpty(wareHouseCode))
            return await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == accountCode && x.ParentRef == parentRef && x.WarehouseCode == wareHouseCode);

        if (!string.IsNullOrEmpty(parentRef))
        {
            return await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == accountCode && x.ParentRef == parentRef);
        }

        return await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == accountCode);
    }

    public async Task<List<ChartOfAccountModel>> GetAllAccountCustomer(int year)
    {
        try
        {
            var query = GetAccountQuery(year);
            query = query.Where(x => x.Type < 5 && x.DisplayInsert
                && x.AccGroup == 2);
            var res = await query
                .OrderBy(x => x.Code)
                .ToListAsync();

            return res;
        }
        catch
        {
            return new List<ChartOfAccountModel>();
        }
    }

    public async Task<List<ChartAccountDropDownViewModel>> GetAllAccountSelections(List<int> classifications, int year)
    {
        var query = await _context.GetChartOfAccount(year).Where(x => x.Type < 5 &&
                        x.DisplayInsert && classifications.Contains(x.Classification))
                        .OrderBy(x => x.Code)
                        .Select(coa => new ChartAccountDropDownViewModel
                        {
                            Code = coa.Code,
                            Id = coa.Id,
                            Name = coa.Name,
                            Type = coa.Type,
                            AccGroup = coa.AccGroup,
                            IsForeignCurrency = coa.IsForeignCurrency,

                            ArisingCredit = coa.ArisingCredit,
                            ArisingDebit = coa.ArisingDebit,
                            OpeningCredit = coa.OpeningCredit,
                            OpeningDebit = coa.OpeningDebit,

                            ArisingForeignCredit = coa.ArisingForeignCredit,
                            ArisingForeignDebit = coa.ArisingForeignDebit,
                            OpeningForeignCredit = coa.OpeningForeignCredit,
                            OpeningForeignDebit = coa.OpeningForeignDebit,

                            ParentRef = coa.ParentRef,
                            WarehouseCode = coa.WarehouseCode,
                            Classification = coa.Classification,
                            ClosingDebit = coa.OpeningDebit - coa.ArisingDebit,
                            ClosingCredit = coa.OpeningCredit - coa.ArisingCredit,
                            ClosingStockQuantity = (coa.OpeningStockQuantity ?? 0) - (coa.ArisingStockQuantity ?? 0),
                            ClosingForeignCredit = coa.OpeningForeignCredit - coa.ArisingForeignCredit,
                            ClosingForeignDebit = coa.OpeningForeignDebit - coa.ArisingForeignDebit,
                            DisplayInsert = coa.DisplayInsert,
                            Duration = coa.Duration,
                            HasChild = coa.HasChild,
                            HasDetails = coa.HasDetails,
                            IsInternal = coa.IsInternal
                        }  /*value*/)
                        .Distinct()
                        .ToListAsync();
        return query;
    }

    public async Task<ObjectReturn> CheckAccountDetail(ChartOfAccount data, int year)
    {
        var account = await _context.GetChartOfAccount(year).FirstOrDefaultAsync(x => x.Code == data.Code && x.WarehouseCode == data.WarehouseCode && x.ParentRef == data.ParentRef);
        if (account != null)
            return new ObjectReturn
            {
                message = ChartOfAccountResultErrorConstants.ACCOUNT_EXIST,
                status = 400,
                code = "ACCOUNT_EXIST",
            };
        string parentCode = data.ParentRef ?? "";
        if (parentCode.Contains(":"))
        {
            parentCode = parentCode.Split(":")[0];
            var ledger = await _context.GetLedger(year).FirstOrDefaultAsync(x => x.CreditDetailCodeSecond == parentCode || x.DebitDetailCodeSecond == parentCode);
            if (ledger != null)
                return new ObjectReturn
                {
                    message = ChartOfAccountResultErrorConstants.ACCOUNT_LEDGER_DETAIL,
                    status = 400,
                };
        }
        else
        {
            var ledger = _context.GetLedger(year).FirstOrDefault(x => x.CreditDetailCodeFirst == parentCode || x.DebitDetailCodeFirst == parentCode);
            if (ledger != null)
                return new ObjectReturn
                {
                    message = ChartOfAccountResultErrorConstants.ACCOUNT_LEDGER_DETAIL,
                    status = 400,
                };
        }
        return new ObjectReturn
        {
            message = "",
            status = 200,
        };
    }

    public async Task<string> CreateAccountGroup(ChartOfAccountGroup accountGroup, int year)
    {
        if (await _context.GetChartOfAccountGroup(year).AnyAsync(x => x.Code == accountGroup.Code))
            return ErrorMessages.AccountGroupCodeAlreadyExist;
        accountGroup.Year = year;
        _context.ChartOfAccountGroups.Add(accountGroup);
        await _context.SaveChangesAsync();
        return string.Empty;
    }

    public async Task<List<ChartOfAccountGroupModel>> GetAllGroups(int year)
    {
        return await (from coa in _context.GetChartOfAccountGroup(year)
                      select new ChartOfAccountGroupModel()
                      {
                          Id = coa.Id,
                          Code = coa.Code,
                          Name = coa.Name,
                          Details = _context.GetChartOfAccountGroupLink(year).Where(x => x.CodeChartOfAccountGroup == coa.Code).Select(x => x.CodeChartOfAccount).ToList(),
                      }).ToListAsync();
    }

    public async Task<string> UpdateGroupDetails(ChartOfAccountGroupModel model, int year)
    {
        try
        {
            await _context.Database.BeginTransactionAsync();

            List<string> itemChecks = await _context.GetChartOfAccountGroupLink(year).Where(x => model.Details.Contains(x.CodeChartOfAccount) && x.CodeChartOfAccountGroup != model.Code).Select(x => x.CodeChartOfAccount).ToListAsync();
            if (itemChecks.Count > 0)
            {
                _context.Database.RollbackTransaction();
                return string.Join(",", itemChecks) + " đã tồn tại";
            }
            List<ChartOfAccountGroupLink> oldDetails = await _context.GetChartOfAccountGroupLink(year).Where(x => x.CodeChartOfAccountGroup == model.Code)
            .ToListAsync();
            _context.ChartOfAccountGroupLinks.RemoveRange(oldDetails);

            _context.ChartOfAccountGroupLinks.AddRange(model.Details.Select(x => new ChartOfAccountGroupLink()
            { CodeChartOfAccountGroup = model.Code, CodeChartOfAccount = x, Year = year }));
            await _context.SaveChangesAsync();
            _context.Database.CommitTransaction();
            return string.Empty;
        }
        catch
        {
            _context.Database.RollbackTransaction();
            return "Lỗi cập nhật";
        }
    }

    public async Task<string> DeleteGroup(string groupId, int year)
    {
        var group = await _context.ChartOfAccountGroups.FindAsync(groupId);
        if (group == null)
            return ErrorMessages.DataNotFound;

        var groupLinks = await _context.GetChartOfAccountGroupLink(year).Where(x => x.CodeChartOfAccountGroup == group.Code).ToListAsync();
        _context.ChartOfAccountGroups.Remove(group);
        _context.ChartOfAccountGroupLinks.RemoveRange(groupLinks);
        await _context.SaveChangesAsync();
        return string.Empty;
    }

    public void UpdateAccount(ChartOfAccount account)
    {
        try
        {
            _context.ChartOfAccounts.Update(account);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public async Task<string> UpdateArisingAccount(int year, string dbName)
    {
        UpdateArisingAccountQueue updateArisingAccountQueue = new()
        {
            CreateAt = DateTime.UtcNow,
            Status = "PREPARE",
            UserId = 0
        };
        await _context.UpdateArisingAccountQueue.AddAsync(updateArisingAccountQueue);
        await _context.SaveChangesAsync();

        
        BackgroundJob.Enqueue(() => _chartOfAccountCaculatorQueue.Perform(year, dbName));

        return string.Empty;
    }

    private void DeQuyUpdateChartOfAccount(List<ChartOfAccount> accounts, ChartOfAccount account)
    {
        string parentRef = account.Code;
        if (account.Type == 5)
        {
            parentRef = account.ParentRef + ":" + account.Code;
        }
        var accountChidrens = accounts.Where(x => x.ParentRef == parentRef
        && (x.WarehouseCode == account.WarehouseCode || string.IsNullOrEmpty(x.WarehouseCode))).ToList();

        foreach (var accountChidren in accountChidrens)
        {
            DeQuyUpdateChartOfAccount(accounts, accountChidren);
        }

        if (accountChidrens.Count > 0)
        {
            account.OpeningDebit = accountChidrens.Sum(x => x.OpeningDebit);
            account.OpeningCredit = accountChidrens.Sum(x => x.OpeningCredit);
            account.OpeningForeignDebit = accountChidrens.Sum(x => x.OpeningForeignDebit);
            account.OpeningForeignCredit = accountChidrens.Sum(x => x.OpeningForeignCredit);

            account.OpeningDebitNB = accountChidrens.Sum(x => x.OpeningDebitNB);
            account.OpeningCreditNB = accountChidrens.Sum(x => x.OpeningCreditNB);
            account.OpeningForeignDebitNB = accountChidrens.Sum(x => x.OpeningForeignDebitNB);
            account.OpeningForeignCreditNB = accountChidrens.Sum(x => x.OpeningForeignCreditNB);
            if (account.Type > 3)
            {
                account.HasChild = false;
                account.HasDetails = true;
                account.DisplayDelete = false;
            }
            else
            {
                account.HasChild = true;
                account.DisplayInsert = false;
                account.DisplayDelete = false;
            }
        }
        if (account.Id > 0)
        {
            _context.ChartOfAccounts.Update(account);
        }
        else
            _context.ChartOfAccounts.Add(account);
    }

    public async Task<string> GetCodeAuto(string parentRef, int isInternal, int year)
    {
        ChartOfAccount account;
        var accounts = await _context.GetChartOfAccount(year).Where(x => x.ParentRef == parentRef && x.IsInternal == isInternal).ToListAsync();
        string prefix = "";
        int lengthCode = parentRef.Contains(":") ? 5 : 4;
        int order = 1;

        if (isInternal == 3)
        {
            prefix = "NB";
        }
        else if (isInternal == 2)
        {
            prefix = "HT";
        }

        if (isInternal == 1)
        {
            if (!parentRef.Contains(":"))
            {
                Regex r = new Regex(@"[0-9]{4}");

                account = accounts.Where(x => r.IsMatch(x.Code)).OrderByDescending(x => x.Code).FirstOrDefault();
            }
            else
            {
                Regex r = new Regex(@"[0-9]{5}");

                account = accounts.Where(x => r.IsMatch(x.Code)).OrderByDescending(x => x.Code).FirstOrDefault();
            }
        }
        else
        {
            if (!parentRef.Contains(":"))
            {
                Regex r = new Regex(@"[N|H][B|T][0-9]{4}");
                account = accounts.Where(x => r.IsMatch(x.Code)).OrderByDescending(x => x.Code.Remove(0, 2)).FirstOrDefault();
            }
            else
            {
                Regex r = new Regex(@"[^NH][BT][0-9]{5}");
                account = accounts.Where(x => r.IsMatch(x.Code)).OrderByDescending(x => x.Code.Remove(0, 2)).FirstOrDefault();
            }
        }
        try
        {
            if (account != null)
            {
                if (account.IsInternal != 1)
                {
                    order = int.Parse(account.Code.Remove(0, 2)) + 1;
                }
                else
                {
                    order = int.Parse(account.Code) + 1;
                }
            }
            string code = order.ToString();
            while (code.Length < lengthCode)
            {
                code = "0" + code;
            }
            return prefix + code;
        }
        catch
        {
            string code = order.ToString();
            while (code.Length < lengthCode)
            {
                code = "0" + code;
            }
            return prefix + code;
        }
    }

    private IQueryable<ChartOfAccountModel> GetAccountQuery(int year)
    {
        return from coa in _context.GetChartOfAccount(year)
               select new ChartOfAccountModel()
               {
                   Code = coa.Code,
                   Duration = coa.Duration,
                   Id = coa.Id,
                   Name = coa.Name,
                   Type = coa.Type,
                   ArisingCredit = coa.ArisingCredit,
                   ArisingDebit = coa.ArisingDebit,
                   DisplayDelete = coa.DisplayDelete,
                   DisplayInsert = coa.DisplayInsert,
                   Protected = coa.Protected,
                   OpeningCredit = coa.OpeningCredit,
                   OpeningDebit = coa.OpeningDebit,
                   ParentRef = coa.ParentRef,
                   ArisingForeignCredit = coa.ArisingForeignCredit,
                   ArisingForeignDebit = coa.ArisingForeignDebit,
                   OpeningForeignCredit = coa.OpeningForeignCredit,
                   OpeningForeignDebit = coa.OpeningForeignDebit,
                   IsForeignCurrency = coa.IsForeignCurrency,
                   Classification = coa.Classification,
                   AccGroup = coa.AccGroup,
                   WarehouseCode = coa.WarehouseCode,
                   OpeningStockQuantity = coa.OpeningStockQuantity,
                   StockUnit = coa.StockUnit,
                   StockUnitPrice = coa.StockUnitPrice,
                   HasChild = coa.HasChild,
                   HasDetails = coa.HasDetails,

                   OpeningStockQuantityNB = coa.OpeningStockQuantityNB,
                   StockUnitPriceNB = coa.StockUnitPriceNB,
                   OpeningDebitNB = coa.OpeningDebitNB,
                   OpeningCreditNB = coa.OpeningCreditNB,
                   OpeningForeignCreditNB = coa.OpeningForeignCreditNB,
                   OpeningForeignDebitNB = coa.OpeningForeignDebitNB,

                   ClosingDebit = (coa.OpeningDebit ?? 0) - (coa.ArisingDebit ?? 0),
                   ClosingCredit = (coa.OpeningCredit ?? 0) - (coa.ArisingCredit ?? 0),
                   ClosingStockQuantity = (coa.AccGroup == 1 || coa.AccGroup == 2) ? 0 : ((coa.OpeningStockQuantity ?? 0) - (coa.ArisingStockQuantity ?? 0)),
                   ClosingAmount = (coa.OpeningDebit ?? 0) - (coa.OpeningCredit ?? 0) + (coa.ArisingDebit ?? 0) - (coa.ArisingCredit ?? 0),
               };
    }

    public async Task TransferAccount(int currentYear, int year)
    {
        var isHaveYear = await _context.YearSales.Where(x => x.Year == year).AnyAsync();
        if (!isHaveYear)
            throw new ErrorException("Không tồn tại năm");
        var accountNextYears = await _context.ChartOfAccounts.Where(x => x.Year == year).ToListAsync();

        var accountChecks = new List<ChartOfAccount>();
        var accounts = await _context.GetChartOfAccount(currentYear).ToListAsync();

        var accountParents = accounts.Where(x => string.IsNullOrEmpty(x.ParentRef)).ToList();
        foreach (var account in accountParents)
        {
            var accountChildrenRecursive = accounts.Where(x => x.ParentRef == account.Code).ToList();
            RecursiveTransferAccount(accounts, accountChildrenRecursive, year, ref accountChecks, accountNextYears, account);
        }
        var accountUpdates = accountChecks.Where(x => x.Id > 0);
        _context.ChartOfAccounts.UpdateRange(accountUpdates);
        var accountAdds = accountChecks.Where(x => x.Id == 0);
        await _context.ChartOfAccounts.AddRangeAsync(accountAdds);

        await _context.SaveChangesAsync();
    }

    private void RecursiveTransferAccount(List<ChartOfAccount> accounts, List<ChartOfAccount> accountParents, int year,
        ref List<ChartOfAccount> accountAdds, List<ChartOfAccount> accountNextYears, ChartOfAccount accountParent)
    {
        if (!accountParents.Any())
        {
            var existAccount = accountNextYears.Find(a => a.Code == accountParent.Code && a.ParentRef == accountParent.ParentRef && a.WarehouseCode == accountParent.WarehouseCode);
            if (existAccount != null)
            {
                accountParent.Id = existAccount.Id;
            }
            else
            {
                accountParent.Id = 0;
            }

            accountParent.Year = year;
            var Residual_Amount = (accountParent.OpeningDebit ?? 0) - (accountParent.OpeningCredit ?? 0) + (accountParent.ArisingDebit ?? 0) - (accountParent.ArisingCredit ?? 0);
            var Residual_AmountNb = (accountParent.OpeningDebitNB ?? 0) - (accountParent.OpeningCreditNB ?? 0) + (accountParent.ArisingDebitNB ?? 0) - (accountParent.ArisingCreditNB ?? 0);
            if (Residual_Amount > 0)
                accountParent.OpeningDebit = Residual_Amount;
            else
                accountParent.OpeningCredit = Residual_Amount;
            if (Residual_AmountNb > 0)
                accountParent.OpeningDebitNB = Residual_AmountNb;
            else
                accountParent.OpeningCreditNB = Residual_AmountNb;
            var OpeningForeign = accountParent.OpeningForeignDebit + accountParent.ArisingForeignDebit - (accountParent.OpeningForeignCredit + accountParent.ArisingForeignCredit);
            var OpeningForeignNB = accountParent.OpeningForeignDebitNB + accountParent.ArisingForeignDebitNB - (accountParent.OpeningForeignCreditNB + accountParent.ArisingForeignCreditNB);
            if (OpeningForeign > 0)
                accountParent.OpeningForeignDebit = OpeningForeign;
            else
                accountParent.OpeningForeignCredit = OpeningForeign;

            if (OpeningForeignNB > 0)
                accountParent.OpeningForeignDebitNB = accountParent.OpeningForeignDebitNB + accountParent.ArisingForeignDebitNB;
            else
                accountParent.OpeningForeignCreditNB = accountParent.OpeningForeignCreditNB + accountParent.ArisingForeignCreditNB;
            accountParent.OpeningStockQuantity = accountParent.OpeningStockQuantity + accountParent.ArisingStockQuantity;
            accountParent.OpeningStockQuantityNB = accountParent.OpeningStockQuantityNB + accountParent.ArisingStockQuantityNB;
            accountAdds.Add(accountParent);
            return;
        }

        foreach (var account in accountParents)
        {
            var parentRef = account.Code;
            if (account.Type == 5)
                parentRef = $"{account.ParentRef}:{account.Code}";

            var accountChildren = accounts.Where(x => x.ParentRef == parentRef).ToList();
            RecursiveTransferAccount(accounts, accountChildren, year, ref accountAdds, accountNextYears, account);
        }
        {
            var existAccount = accountNextYears.Find(a => a.Code == accountParent.Code && a.ParentRef == accountParent.ParentRef && a.WarehouseCode == accountParent.WarehouseCode);
            if (existAccount != null)
            {
                accountParent.Id = existAccount.Id;
            }
            else
            {
                accountParent.Id = 0;
            }

            accountParent.Year = year;
            accountParent.OpeningDebit = accountParents.Sum(x => x.OpeningDebit);
            accountParent.OpeningCredit = accountParents.Sum(x => x.OpeningCredit);
            accountParent.OpeningDebitNB = accountParents.Sum(x => x.OpeningDebitNB);
            accountParent.OpeningCreditNB = accountParents.Sum(x => x.OpeningCreditNB);
            accountParent.OpeningForeignDebit = accountParents.Sum(x => x.OpeningForeignDebit);
            accountParent.OpeningForeignCredit = accountParents.Sum(x => x.OpeningForeignCredit);
            accountParent.OpeningForeignDebitNB = accountParents.Sum(x => x.OpeningForeignDebitNB);
            accountParent.OpeningForeignCreditNB = accountParents.Sum(x => x.OpeningForeignCreditNB);
            accountParent.OpeningStockQuantity = accountParents.Sum(x => x.OpeningStockQuantity);
            accountParent.OpeningStockQuantityNB = accountParents.Sum(x => x.OpeningStockQuantityNB);
            accountAdds.Add(accountParent);
        }
    }
}