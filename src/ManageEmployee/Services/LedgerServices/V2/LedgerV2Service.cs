using AutoMapper;
using Common.Constants;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.LedgerWarehouseModels;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.ProjectModels;
using ManageEmployee.DataTransferObject.V2;
using ManageEmployee.Entities.ChartOfAccountEntities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Services.Interfaces.Ledgers.V2;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ManageEmployee.Services.LedgerServices.V2;

public class LedgerV2Service : ILedgerV2Service
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public LedgerV2Service(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagingResultLedger<LedgerV2Model>> GetPage(LedgerRequestModel request, int yearFilter)
    {
        var searchQuery = string.IsNullOrWhiteSpace(request.SearchText) ? "" : request.SearchText.Trim().ToLower();
        if (request.Page == 0)
        {
            request.Page = 1;
        }

        var response = new PagingResultLedger<LedgerV2Model>()
        {
            CurrentPage = request.Page,
            PageSize = request.PageSize,
        };

        var listType = new List<string>();
        if (request.DocumentType == nameof(AssetsType.PB))
        {
            listType.Add(AssetsType.PB.ToString());
            listType.Add(AssetsType.CCDCSD.ToString());
        }
        else if (!string.IsNullOrEmpty(request.DocumentType))
        {
            listType.Add(request.DocumentType);
        }
        if(request.IsInternal==0)
        {
            request.IsInternal = 1;
        }    
        IQueryable<LedgerV2Model> query = _context.GetLedger(yearFilter, request.IsInternal)
                    .Select(x => new LedgerV2Model
                    {
                        Id = x.Id,
                        OrginalVoucherNumber = x.OrginalVoucherNumber,
                        Type = x.Type,
                        Month = x.Month,
                        DebitCode = x.DebitCode,
                        DebitDetailCodeFirst = x.DebitDetailCodeFirst,
                        DebitDetailCodeSecond = x.DebitDetailCodeSecond,
                        DebitCodeName = x.DebitCodeName,
                        DebitDetailCodeFirstName = x.DebitDetailCodeFirstName,
                        DebitDetailCodeSecondName = x.DebitDetailCodeSecondName,
                        CreditCode = x.CreditCode,
                        CreditDetailCodeFirst = x.CreditDetailCodeFirst,
                        CreditDetailCodeSecond = x.CreditDetailCodeSecond,
                        CreditCodeName = x.CreditCodeName,
                        CreditDetailCodeFirstName = x.CreditDetailCodeFirstName,
                        CreditDetailCodeSecondName = x.CreditDetailCodeSecondName,
                        OrginalBookDate = x.OrginalBookDate,
                        OrginalDescription = x.OrginalDescription,
                        InvoiceNumber = x.InvoiceNumber,
                        InvoiceTaxCode = x.InvoiceTaxCode,
                        VoucherNumber = x.VoucherNumber,
                        Quantity = x.Quantity,
                        UnitPrice = x.UnitPrice,
                        Amount = x.Amount,
                        InvoiceDate = x.InvoiceDate,
                        Order = x.Order,
                        OrginalCurrency = x.OrginalCurrency,
                        ExchangeRate = x.ExchangeRate,
                        InvoiceCode = x.InvoiceCode,
                        IsInternal = x.IsInternal,
                        BillId = x.BillId,
                    });
        response.NextStt = await _context.GetLedger(yearFilter, request.IsInternal).OrderByDescending(x => x.Order)
            .Where(x => x.Month == request.FilterMonth
            && (listType.Count == 0 ? x.Type == "PT" : listType.Contains(x.Type)))
            .Select(x => x.Order).FirstOrDefaultAsync() + 1;

        if (!string.IsNullOrEmpty(request.DocumentType))
        {
            query = query.Where(l => listType.Contains(l.Type));
        }

        if (request.FilterMonth > 0)
        {
            query = query.Where(l => l.Month == request.FilterMonth);
        }

        if (!string.IsNullOrEmpty(searchQuery))
        {
            if (string.IsNullOrEmpty(request.DocumentType) && searchQuery.ToLower().Contains("chứng từ kèm"))
            {
                query = query.Where(x => x.BillId != null);
            }
            else
            {
                query = query.Where(x => x.OrginalVoucherNumber.Contains(searchQuery)
                                         || x.DebitCode.ToLower().Contains(searchQuery) ||
                                         x.DebitCodeName.ToLower().Contains(searchQuery)
                                         || x.DebitDetailCodeFirst.ToLower().Contains(searchQuery) ||
                                         x.DebitDetailCodeFirstName.ToLower().Contains(searchQuery)
                                         || x.DebitDetailCodeSecond.ToLower().Contains(searchQuery) ||
                                         x.DebitDetailCodeSecondName.ToLower().Contains(searchQuery)
                                         || x.CreditCode.ToLower().Contains(searchQuery) ||
                                         x.CreditCodeName.ToLower().Contains(searchQuery)
                                         || x.CreditDetailCodeFirst.ToLower().Contains(searchQuery) ||
                                         x.CreditDetailCodeFirstName.ToLower().Contains(searchQuery)
                                         || x.CreditDetailCodeSecond.ToLower().Contains(searchQuery) ||
                                         x.CreditDetailCodeSecondName.ToLower().Contains(searchQuery)
                                         || x.OrginalDescription.ToLower().Contains(searchQuery)
                                         || x.InvoiceNumber.Contains(searchQuery)
                                         || x.InvoiceTaxCode.Contains(searchQuery)
                                         || x.VoucherNumber.Contains(searchQuery));
            }
        }
        
        response.TotalItems = await query.CountAsync();
        var data = await query.OrderByDescending(s => s.Order).ThenByDescending(s => s.Id)
            .Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync();

        response.Data = await TotalAmount(data, request.IsInternal, yearFilter);

        return response;
    }

    private async Task<List<LedgerV2Model>> TotalAmount(List<LedgerV2Model> ledgers, int isInternal, int yearFilter)
    {
        var orginalVoucherNumbers = ledgers.Select(x => x.OrginalVoucherNumber).Distinct();
        var ledgerFinds = await _context.GetLedger(yearFilter, isInternal).Where(x => orginalVoucherNumbers.Contains(x.OrginalVoucherNumber))
            .ToListAsync();

        foreach (var ledger in ledgers)
        {
            ledger.TotalAmount = ledgerFinds.Where(x => x.OrginalVoucherNumber == ledger.OrginalVoucherNumber)
                .Sum(x => x.Amount);
        }

        return ledgers;
    }

    public async Task<LedgerDetailV2Model> GetByIdAsync(long id, int isInternal, int year)
    {
        Ledger query = null;

        query = await _context.GetLedger(year, isInternal).AsNoTracking()
            .FirstOrDefaultAsync(f => f.Id == id);

        if (query is null)
        {
            return null;
        }
        var result = _mapper.Map<LedgerDetailV2Model>(query);

        var codes = new string[]
        {
            query.CreditCode, query.CreditDetailCodeFirst, query.CreditDetailCodeSecond,
            query.DebitCode, query.DebitDetailCodeFirst, query.DebitDetailCodeSecond,
        }.Where(w => !string.IsNullOrEmpty(w)).ToArray();
        if (!codes.Any())
        {
            return result;
        }
        var warehouseCodes = new string[]
       {
            query.CreditWarehouse,
            query.DebitWarehouse,
       }.Where(w => !string.IsNullOrEmpty(w)).ToArray();

        var accounts = await _context.GetChartOfAccount(year).AsNoTracking().Where(x => codes.Contains(x.Code) && (string.IsNullOrEmpty(x.WarehouseCode) || warehouseCodes.Contains(x.WarehouseCode)))
            .ToListAsync();
        if (!accounts.Any())
        {
            return result;
        }

        result.Debit = FindAccount(accounts, query.DebitCode, string.Empty, query.DebitWarehouse);
        result.DebitDetailFirst = FindAccount(accounts, query.DebitDetailCodeFirst, query.DebitCode, query.DebitWarehouse);
        result.DebitDetailSecond = FindAccount(accounts, query.DebitDetailCodeSecond, query.DebitCode + ":" + query.DebitDetailCodeFirst, query.DebitWarehouse);
        result.Credit = FindAccount(accounts, query.CreditCode, string.Empty, query.CreditWarehouse);
        result.CreditDetailFirst = FindAccount(accounts, query.CreditDetailCodeFirst, query.CreditCode, query.CreditWarehouse);
        result.CreditDetailSecond = FindAccount(accounts, query.CreditDetailCodeSecond, query.CreditCode + ":" + query.CreditDetailCodeFirst, query.CreditWarehouse);
        double totalAmount = 0;

        totalAmount = await _context.GetLedger(year, isInternal).Where(x => x.OrginalVoucherNumber == result.OrginalVoucherNumber && x.OrginalBookDate == result.OrginalBookDate).SumAsync(f => f.Amount);
        result.TotalAmount = totalAmount;


        // if (result.Debit is { HasDetails: true })
        // {
        //     result.DebitDetailFirst = FindAccount(accounts, query.DebitDetailCodeFirst);
        //     if (result.DebitDetailFirst is { HasDetails: true })
        //     {
        //         result.DebitDetailSecond = FindAccount(accounts, query.DebitDetailCodeSecond);
        //     }
        // }
        //
        // if (result.Credit is not { HasDetails: true }) return result;
        // result.CreditDetailFirst = FindAccount(accounts, query.CreditDetailCodeFirst);
        // if (result.CreditDetailFirst is { HasDetails: true })
        // {
        //     result.CreditDetailSecond = FindAccount(accounts, query.CreditDetailCodeSecond);
        // }
        var taxRate = await _context.TaxRates.FirstOrDefaultAsync(x => x.Code == query.InvoiceCode);
        result.InvoicePercent = taxRate?.Percent;
        result.Project = await _context.Projects.Where(x => x.Code == query.ProjectCode).Select(x => new ProjectModel
        {
            Id = x.Id,
            Code = x.Code,
            Name = x.Name,
        }).FirstOrDefaultAsync();
        return result;
    }

    private CommonModel FindAccount(List<ChartOfAccount> accounts, string code, string parentRef, string warehouseCode)
    {
        if (!accounts.Any())
        {
            return null;
        }
        return accounts.Where(f => f.Code == code 
                                    && (string.IsNullOrEmpty(parentRef) || f.ParentRef == parentRef)
                                    && (string.IsNullOrEmpty(f.WarehouseCode) || f.WarehouseCode == warehouseCode)).Select(account => new CommonModel
        {
            Id = account.Id,
            Code = account.Code,
            Name = account.Name,
            IsForeignCurrency = account.IsForeignCurrency,
            AccGroup = account.AccGroup,
            Classification = account.Classification,
            HasDetails = account.HasDetails,
            WarehouseCode = account.WarehouseCode,
            DisplayInsert = account.DisplayInsert,
            Duration = account.Duration,
            Protected = account.Protected,
            ParentRef = account.ParentRef,

        }).FirstOrDefault();
    }

    public async Task<TotalArisingForVoucherNumberModel> TotalArisingForVoucherNumber(string orginalVoucherNumber, int isInternal)
    {
        TotalArisingForVoucherNumberModel itemOut = new();
        List<Ledger> ledgers = await _context.GetLedgerNotForYear(isInternal).Where(x => x.IsInternal != LedgerInternalConst.LedgerTemporary && x.OrginalVoucherNumber == orginalVoucherNumber)
                .Select(x => new Ledger
                {
                    CreditCode = x.CreditCode,
                    DebitCode = x.DebitCode,
                    Amount = x.Amount,
                })
                .ToListAsync();


        if (ledgers.Count > 0)
        {
            itemOut.TotalItem = ledgers.Count;
            itemOut.TotalAmount = ledgers.Sum(x => x.Amount);
            var accountCodes = ledgers.Select(x => x.DebitCode).Concat(ledgers.Select(x => x.CreditCode)).Distinct();
            foreach (var accountCode in accountCodes)
            {
                LedgerArisingModel ledger = new();
                ledger.TotalAmount = ledgers.Where(x => x.CreditCode == accountCode || x.DebitCode == accountCode).Sum(x => x.Amount);
                ledger.AcountCode = accountCode;
                itemOut.ledgers.Add(ledger);
            }
        }
        return itemOut;
    }
}
public class TotalArisingForVoucherNumberModel
{
public int TotalItem { get; set; }
public double TotalAmount { get; set; }
public List<LedgerArisingModel> ledgers { get; set; } = new List<LedgerArisingModel>();

}
public class LedgerArisingModel
{
public string? AcountCode { get; set; }
public double TotalAmount { get; set; }
}