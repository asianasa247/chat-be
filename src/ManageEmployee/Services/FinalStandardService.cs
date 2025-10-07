using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.ChartOfAccountModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Accounts;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using ManageEmployee.Services.Interfaces.FinalStandards;
using ManageEmployee.Services.Interfaces.Ledgers;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services;

public class FinalStandardService : IFinalStandardService
{
    private readonly ApplicationDbContext _context;
    private readonly IChartOfAccountService _chartOfAccountService;
    private readonly IAccountBalanceSheetService _accountBalanceSheetService;
    private readonly ILedgerHelperService _ledgerHelperService;
    private readonly ILedgerService _ledgerService;

    public FinalStandardService(ApplicationDbContext context, IChartOfAccountService chartOfAccountService,
        IAccountBalanceSheetService accountBalanceSheetService, ILedgerHelperService ledgerHelperService, ILedgerService ledgerService)
    {
        _context = context;
        _chartOfAccountService = chartOfAccountService;
        _accountBalanceSheetService = accountBalanceSheetService;
        _ledgerHelperService = ledgerHelperService;
        _ledgerService = ledgerService;
    }

    public async Task<PagingResult<FinalStandard>> GetPaging(PagingRequestModel param)
    {
        var query = (from p in _context.FinalStandards
                     where p.IsDelete != true
                     //&& (!String.IsNullOrEmpty(keyword) ? (
                     //p.CreditCode.Trim().Contains(keyword) || p.CreditCode.Trim().StartsWith(keyword) || p.CreditCode.Trim().EndsWith(keyword) ||
                     //p.DebitCode.Trim().Contains(keyword) || p.DebitCode.Trim().StartsWith(keyword) || p.DebitCode.Trim().EndsWith(keyword)
                     //) : p.Id != 0)
                     select p).OrderBy(x => x.DebitCode).ThenBy(x => x.CreditCode);
        return new PagingResult<FinalStandard>
        {
            CurrentPage = param.Page,
            PageSize = param.PageSize,
            TotalItems = await query.CountAsync(),
            Data = await query.Skip(param.PageSize * (param.Page - 1)).Take(param.PageSize).ToListAsync()
        };
    }

    public FinalStandard GetById(int Id)
    {
        return _context.FinalStandards.Find(Id);
    }

    public FinalStandard Create(FinalStandard param)
    {
        try
        {
            if (string.IsNullOrEmpty(param.CreditCode) || string.IsNullOrEmpty(param.DebitCode))
                throw new ErrorException(ResultErrorConstants.MODEL_MISS);
            param.IsDelete = false;
            _context.FinalStandards.Add(param);
            _context.SaveChanges();

            return param;
        }
        catch
        {
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);
        }
    }

    public void Update(FinalStandard param)
    {
        var documentCurrent = _context.FinalStandards.Find(param.Id);

        if (documentCurrent == null)
            throw new ErrorException(ResultErrorConstants.MODEL_NULL);

        documentCurrent.DebitCode = param.DebitCode;
        documentCurrent.CreditCode = param.CreditCode;
        documentCurrent.PercentRatio = param.PercentRatio;
        documentCurrent.Type = param.Type;

        _context.FinalStandards.Update(documentCurrent);
        _context.SaveChanges();
    }

    public IEnumerable<FinalStandard> GetAll()
    {
        return _context.FinalStandards
            .Where(x => !x.IsDelete);
    }

    public async Task Delete(int id)
    {
        var documentCurrent = await _context.FinalStandards.FindAsync(id);
        if (documentCurrent != null)
        {
            documentCurrent.IsDelete = true;
            documentCurrent.DeleteAt = DateTime.Now;
            _context.FinalStandards.Update(documentCurrent);
            _context.SaveChanges();
        }
    }

    public async Task<List<FinalStandardDetailModel>> GetFinalStandardDetail(PagingationFinalStandardRequestModel filter, int year)
    {
        var result = new List<FinalStandardDetailModel>();

        if (filter.ListIDFinal == null || !filter.ListIDFinal.Any())
            throw new ErrorException(ResultErrorConstants.OBJ_NULL);

        var from = new DateTime(year, DateTime.Now.Month, 1);
        var to = from.AddMonths(1).AddSeconds(-1);

        var items = await _accountBalanceSheetService.GenerateReport(from, to, year);
        var listAccountBalance = items
            .GroupBy(x => x.Hash)
            .Select(x => new AccountBalanceItemViewModel()
            {
                AccountCode = x.First().AccountCode,
                IsForeign = x.First().IsForeign,
                AccountName = x.First().AccountName,
                AccountType = x.First().AccountType,
                OpeningDebit = x.Sum(s => s.OpeningDebit),
                OpeningCredit = x.Sum(s => s.OpeningCredit),
                ArisingDebit = x.Sum(s => s.ArisingDebit),
                ArisingCredit = x.Sum(s => s.ArisingCredit),
                CumulativeDebit = x.Sum(s => s.CumulativeDebit),
                CumulativeCredit = x.Sum(s => s.CumulativeCredit),
                ClosingDebit = x.Sum(s => s.ClosingDebit),
                ClosingCredit = x.Sum(s => s.ClosingCredit),
                ParentRef=x.First().ParentRef,

                Hash = x.Key
            }).ToList();

        var finalStandards = await _context.FinalStandards.Where(x => !x.IsDelete && filter.ListIDFinal.Contains(x.Id)).OrderBy(x => x.DebitCode).ThenBy(x => x.CreditCode).ToListAsync();

        foreach (var finalStandard in finalStandards)
        {
            var listDetail1 = await _chartOfAccountService.GetAllDetails(0, 20, finalStandard.DebitCode, string.Empty, string.Empty, year);
            if (listDetail1.Any())
            {
                List<ChartOfAccountModel> listDetail2Credit = new List<ChartOfAccountModel>();
                var listDetail1Credit = await _chartOfAccountService.GetAllDetails(0, 20, finalStandard.CreditCode, string.Empty, string.Empty, year);
                if (listDetail1Credit.Any())
                {
                    foreach (var detail1 in listDetail1Credit)
                    {
                        var listDetail2 = await _chartOfAccountService.GetAllDetails(0, 20, $"{finalStandard.CreditCode}:{detail1.Code}", detail1.WarehouseCode, string.Empty, year);
                        if (listDetail2.Any())
                        {
                            listDetail2Credit.AddRange(listDetail2);
                        }
                    }
                }
                var listCodeDebit1 = listDetail1.Select(x => x.Code).ToList();
                var creditDetail1 = listDetail1Credit.FirstOrDefault(x => !listCodeDebit1.Contains(x.Code));
                foreach (var detail1 in listDetail1)
                {
                    var creditDetail1_check = listDetail1Credit.FirstOrDefault(x => x.Code == detail1.Code);
                    if (creditDetail1_check == null)
                        creditDetail1_check = creditDetail1;
                    string parentRef = $"{finalStandard.DebitCode}:{detail1.Code}";

                    var listDetail2 = await _chartOfAccountService.GetAllDetails(0, 20, parentRef, detail1.WarehouseCode, string.Empty, year);
                    if (listDetail2.Any())
                    {

                        var listCodeDebit2 = listDetail2.Select(x => x.Code).ToList();
                        var creditDetail2 = listDetail2Credit.FirstOrDefault(x => !listCodeDebit2.Contains(x.Code));

                        foreach (var detail2 in listDetail2)
                        {
                            var creditDetail2_check = listDetail2Credit.FirstOrDefault(x => x.Code == detail2.Code);
                            if (creditDetail2_check == null)
                                creditDetail2_check = creditDetail2;

                            var model = new FinalStandardDetailModel()
                            {
                                DebitCode = finalStandard.DebitCode,
                                DebitCodeDetail1 = detail1.Code,
                                DebitCodeDetail2 = detail2.Code,
                                DebitCodeWareHouse = detail2.WarehouseCode,
                                CreditCode = finalStandard.CreditCode,
                                CreditCodeDetail1 = creditDetail1_check?.Code,
                                CreditCodeDetail2 = creditDetail2_check?.Code,
                                CreditCodeWareHouse = creditDetail1_check?.WarehouseCode,
                                PercentRatio = finalStandard.PercentRatio,
                                Type = finalStandard.Type,
                                Amount = (finalStandard.Type == nameof(FinalStandardTypeEnum.debitToCredit)) ? listAccountBalance.FirstOrDefault(x => x.AccountCode == detail2.Code && x.ParentRef == parentRef)?.ClosingDebit : listAccountBalance.FirstOrDefault(x => x.AccountCode == detail2.Code && x.ParentRef == parentRef)?.ClosingCredit
                            };
                            if (filter.IsAllFinalStandar == false && model.Amount == 0)
                                continue;

                            result.Add(model);
                        }
                    }
                    else
                    {
                        var model = new FinalStandardDetailModel()
                        {
                            DebitCode = finalStandard.DebitCode,
                            DebitCodeDetail1 = detail1.Code,
                            DebitCodeDetail2 = null,
                            DebitCodeWareHouse = detail1.WarehouseCode,
                            CreditCode = finalStandard.CreditCode,
                            CreditCodeDetail1 = creditDetail1_check?.Code,
                            CreditCodeDetail2 = null,
                            CreditCodeWareHouse = creditDetail1_check?.WarehouseCode,
                            PercentRatio = finalStandard.PercentRatio,
                            Type = finalStandard.Type,
                            Amount = (finalStandard.Type == nameof(FinalStandardTypeEnum.debitToCredit)) ? listAccountBalance.FirstOrDefault(x => x.AccountCode == detail1.Code)?.ClosingDebit : listAccountBalance.FirstOrDefault(x => x.AccountCode == detail1.Code)?.ClosingCredit
                        };
                        if (filter.IsAllFinalStandar == false && model.Amount == 0)
                            continue;
                        result.Add(model);
                    }
                }
            }
            else
            {
                var model = new FinalStandardDetailModel()
                {
                    DebitCode = finalStandard.DebitCode,
                    DebitCodeDetail1 = null,
                    DebitCodeDetail2 = null,
                    DebitCodeWareHouse = null,
                    CreditCode = finalStandard.CreditCode,
                    CreditCodeDetail1 = null,
                    CreditCodeDetail2 = null,
                    CreditCodeWareHouse = null,
                    PercentRatio = finalStandard.PercentRatio,
                    Type = finalStandard.Type,
                    Amount = (finalStandard.Type == nameof(FinalStandardTypeEnum.debitToCredit)) ? listAccountBalance.FirstOrDefault(x => x.AccountCode == finalStandard.DebitCode)?.ClosingDebit : listAccountBalance.FirstOrDefault(x => x.AccountCode == finalStandard.DebitCode)?.ClosingCredit
                };
                if (filter.IsAllFinalStandar == false && model.Amount == 0)
                    continue;
                result.Add(model);
            }
        }

        return result;
    }

    public async Task SetIntoLedger(List<FinalStandardDetailModel> finalStandards, int isInternal, int year)
    {
        string typePayLedger = "KC";
        int monthLed = DateTime.Today.Month;
        string voucherMonth = monthLed < 10 ? "0" + monthLed : monthLed.ToString();
        int maxOriginalVoucher = await _ledgerHelperService.GetOriginalVoucher(isInternal != 3, typePayLedger, year);
        var orderString = LedgerHelper.GetOriginalVoucher(maxOriginalVoucher);
        string orginalVoucherNumber = $"{typePayLedger}{voucherMonth}-{DateTime.Today.Year.ToString().Substring(2, 2)}-{orderString}";
        var ledgers = new List<Ledger>();

        foreach (var finalStandard in finalStandards)
        {
            var ledger = LedgerHelper.LedgerInit();
            ledger.Amount = finalStandard.Amount ?? 0;
            ledger.OrginalDescription = $"Kết chuyển {finalStandard.DebitCode} sang tài khoản {finalStandard.CreditCode}";
            ledger.Type = typePayLedger;
            ledger.OrginalVoucherNumber = orginalVoucherNumber;
            ledger.IsInternal = isInternal;
            ledger.Order = maxOriginalVoucher;

            if (finalStandard.Type == nameof(FinalStandardTypeEnum.debitToCredit))
            {
                ledger.DebitCode = finalStandard.CreditCode;
                ledger.DebitDetailCodeFirst = finalStandard.CreditCodeDetail1;
                ledger.DebitDetailCodeSecond = finalStandard.CreditCodeDetail2;
                ledger.CreditCode = finalStandard.DebitCode;
                ledger.CreditDetailCodeFirst = finalStandard.DebitCodeDetail1;
                ledger.CreditDetailCodeSecond = finalStandard.DebitCodeDetail2;
            }
            else
            {
                ledger.DebitCode = finalStandard.DebitCode;
                ledger.DebitDetailCodeFirst = finalStandard.DebitCodeDetail1;
                ledger.DebitDetailCodeSecond = finalStandard.DebitCodeDetail2;
                ledger.CreditCode = finalStandard.CreditCode;
                ledger.CreditDetailCodeFirst = finalStandard.CreditCodeDetail1;
                ledger.CreditDetailCodeSecond = finalStandard.CreditCodeDetail2;
            }

            if (ledger.DebitCode != null && ledger.CreditCode != null)
            {
                await _ledgerService.Create(ledger, year);
            }
        }
    }
}