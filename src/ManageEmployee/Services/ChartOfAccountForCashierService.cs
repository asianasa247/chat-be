using AutoMapper;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.ChartOfAccountModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using ManageEmployee.Services.Interfaces.Companies;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;
public class ChartOfAccountForCashierService: IChartOfAccountForCashierService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICompanyService _companyService;
    public ChartOfAccountForCashierService(ApplicationDbContext context, IMapper mapper, ICompanyService companyService)
    {
        _context = context;
        _mapper = mapper;
        _companyService = companyService;
    }
    public async Task<PagingResult<ChartOfAccountForCashier>> GetPagingChartOfAccountForCashier(ChartOfAccountForCashierPagingRequestModel param, int year)
    {
        if (param.Page < 1)
            param.Page = 1;
        var accountCodeParent = await _context.ChartOfAccountFilters.FirstOrDefaultAsync(x => x.Id == param.ChartOfAccountFilterId);
        if (accountCodeParent == null)
        {
            throw new ErrorException("");
        }
        if (string.IsNullOrEmpty(accountCodeParent.Accounts))
        {
            throw new ErrorException("");
        }
        
        if (!string.IsNullOrEmpty(param.SearchText))
            param.SearchText = param.SearchText.ToLower();

        var accountCodeParents = accountCodeParent.Accounts.Split(";").Where(x => !string.IsNullOrEmpty(x)).ToList();
        // get accounts
        var accounts = await _context.GetChartOfAccount(year).Where(x => !x.HasDetails && !x.HasChild
                    && (string.IsNullOrEmpty(param.SearchText) || x.Code.ToLower().Contains(param.SearchText) || x.Name.ToLower().Contains(param.SearchText))).ToListAsync();
        accounts = accounts.Where(x => accountCodeParents.Exists(y => x.ParentRef.Contains(y))).ToList();
        
        if (!string.IsNullOrEmpty(param.Warehouse))
        {
            accounts = accounts.Where(x => x.WarehouseCode == param.Warehouse).ToList();
        }
        if (!string.IsNullOrEmpty(param.Detail1))
        {
            accounts = accounts.Where(x => x.ParentRef.Contains(param.Detail1) || x.Code == param.Detail1).ToList();
        }

        var company = await _companyService.GetCompany();
        var data = accounts.Skip((param.Page - 1) * param.PageSize).Take(param.PageSize).ToList();
        var itemOut = new List<ChartOfAccountForCashier>();

        foreach (var account in data)
        {
            var item = _mapper.Map<ChartOfAccountForCashier>(account);
            item.Image = company.FileLogo;
            item.Quantity = (account.OpeningStockQuantityNB ?? 0) + (account.ArisingStockQuantityNB ?? 0);
            var detail2 = "";
            var detail1 = account.Code;
            var accountCode = account.ParentRef;
            if (account.ParentRef.Contains(":"))
            {
                detail2 = account.Code;
                detail1 = account.ParentRef.Split(":")[1];
                accountCode = account.ParentRef.Split(":")[0];
            }
            item.Net = await _context.Goods.Where(x => x.PriceList == "BGC"
                            && (string.IsNullOrEmpty(detail2) || x.Detail2 == detail2)
                            && x.Detail1 == detail1
                            && x.Account == accountCode)
                .Select(x => x.Net).FirstOrDefaultAsync();

            itemOut.Add(item);
        }
        
        return new PagingResult<ChartOfAccountForCashier>
        {
            TotalItems = accounts.Count,
            Data = itemOut,
            CurrentPage = param.Page,
            PageSize = param.PageSize,
        };
    }
}
