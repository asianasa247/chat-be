using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.Services.Interfaces.Accounts;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.Ledgers.V2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountBalanceSheetReportController : ControllerBase
{
    private readonly IAccountBalanceSheetService _accountBalanceSheetService;
    private readonly ICompanyService _companyService;
    private readonly IAccountBalanceSheetV2Service _accountBalanceSheetV2Service;

    public AccountBalanceSheetReportController(
        IAccountBalanceSheetService accountBalanceSheetService,
        ICompanyService companyService,
        IAccountBalanceSheetV2Service accountBalanceSheetV2Service)
    {
        _accountBalanceSheetService = accountBalanceSheetService;
        _companyService = companyService;
        _accountBalanceSheetV2Service = accountBalanceSheetV2Service;
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromHeader] int yearFilter, int? fromMonth, int? toMonth, DateTime? fromDate, DateTime? toDate, string accountCode, string preparedBy, int printType)
    {
        var from = new DateTime(yearFilter, 1, 1);
        var to = new DateTime(yearFilter, 12, 31, 23, 59, 59);

        if (fromDate.HasValue && toDate.HasValue)
        {
            if (fromDate.Value > toDate.Value)
            {
                return Ok(new BaseResponseModel()
                {
                    Data = new
                    {
                        Error = "Từ ngày phải nhỏ hơn hoặc bằng tới ngày !"
                    }
                });
            }
            from = fromDate.Value;
            to = toDate.Value;
        }
        else
        {
            if (fromMonth.HasValue && toMonth.HasValue)
            {
                int f = fromMonth.Value <= 0 ? 1 : fromMonth.Value;
                if (f > 12) f = 12;

                int t = toMonth.Value > 12 ? 12 : toMonth.Value;
                if (t <= 0) t = 1;

                if (f > t)
                {
                    return Ok(new BaseResponseModel()
                    {
                        Data = new
                        {
                            Error = "Giá trị từ ngày đến ngày không hợp lệ !"
                        }
                    });
                }

                from = new DateTime(yearFilter, f, 1);
                var lastDate = DateTime.DaysInMonth(yearFilter, t);
                to = new DateTime(yearFilter, t, lastDate, 23, 59, 59);
            }
        }

        var model = new AccountBalanceViewModel();
        var company = await _companyService.GetCompany();
        if (company != null)
        {
            model.Address = company.Address;
            model.Company = company.Name;
            model.TaxId = company.MST;
        }
        model.PreparedBy = preparedBy;
        var items = await _accountBalanceSheetService.GenerateReport(from, to, yearFilter, accountCode);
        model.Items = items
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
                Hash = x.Key
            }).ToList();
        // 1. In cả cột luỹ kế phát sinh
        // 2. Không in những dòng không có số liệu
        if (printType == 2)
        {
            model.Items = model.Items.Where(x =>
                x.OpeningCredit > 0
                || x.OpeningDebit > 0
                || x.ArisingCredit > 0
                || x.ArisingDebit > 0
                || x.ClosingCredit > 0
                || x.ClosingDebit > 0).ToList();
        }
        // 3. In toàn bộ các cấp tiểu khoản và chi tiết
        return Ok(new BaseResponseModel()
        {
            Data = model,
        });
    }

    /// <summary>
    ///  Get Account Infor by Period
    /// </summary>
    /// <param name="fromMonth"></param>
    /// <param name="toMonth"></param>
    /// <param name="fromDate"></param>
    /// <param name="toDate"></param>
    /// <param name="accountCode"></param>
    /// <param name="detailOne"></param>
    /// <param name="detailTwo"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("get_account_info_by_period")]
    public IActionResult GetAccountInfoByPeriod([FromHeader] int yearFilter, int? fromMonth, int? toMonth, DateTime? fromDate, DateTime? toDate, string accountCode, string? detailOne = "", string? detailTwo = "")
    {
        var from = new DateTime(yearFilter, 1, 1);
        var to = new DateTime(yearFilter, 12, 31, 23, 59, 59);

        if (fromDate.HasValue && toDate.HasValue)
        {
            if (fromDate.Value > toDate.Value)
            {
                return Ok(new BaseResponseModel()
                {
                    Data = new
                    {
                        Error = "Từ ngày phải nhỏ hơn hoặc bằng tới ngày !"
                    }
                });
            }
            from = fromDate.Value;
            to = toDate.Value;
        }
        else
        {
            if (fromMonth.HasValue && toMonth.HasValue)
            {
                int f = fromMonth.Value <= 0 ? 1 : fromMonth.Value;
                if (f > 12) f = 12;

                int t = toMonth.Value > 12 ? 12 : toMonth.Value;
                if (t <= 0) t = 1;

                if (f > t)
                {
                    return Ok(new BaseResponseModel()
                    {
                        Data = new
                        {
                            Error = "Giá trị từ ngày đến ngày không hợp lệ !"
                        }
                    });
                }

                from = new DateTime(yearFilter, f, 1);
                var lastDate = DateTime.DaysInMonth(yearFilter, t);
                to = new DateTime(yearFilter, t, lastDate, 23, 59, 59);
            }
        }

        var model = _accountBalanceSheetService.GetAccountInfoByPeriod(from, to, accountCode, yearFilter, detailOne, detailTwo);

        return Ok(new BaseResponseModel()
        {
            Data = model,
        });
    }

    [HttpGet("get-accrual-accounting")]
    public async Task<IActionResult> GetListAccrualAccounting(string type, DateTime fromDate, DateTime toDate,
        string accountCode, string detail1Code, string detail2Code)
    {
        try
        {
            if (type == "date" && fromDate > toDate)
            {
                return Ok(new BaseResponseModel()
                {
                    Data = new
                    {
                        Error = "Từ ngày phải nhỏ hơn hoặc bằng tới ngày !"
                    }
                });
            }

            if (type == "month" && fromDate.Month > toDate.Month)
            {
                return Ok(new BaseResponseModel()
                {
                    Data = new
                    {
                        Error = "Từ tháng phải nhỏ hơn hoặc bằng tới tháng !"
                    }
                });
            }

            var accrualAccounting = await _accountBalanceSheetService.GenerateAccrualAccounting(
                    type, fromDate, toDate, accountCode, detail1Code, detail2Code, false);
            return Ok(new BaseResponseModel()
            {
                Data = accrualAccounting
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("export-data")]
    public async Task<IActionResult> ExportData([FromQuery] AccountBalanceReportParam param, [FromHeader] int yearFilter)
    {
        var response = await _accountBalanceSheetV2Service.GenerateReport(param, yearFilter);
        return Ok(new BaseResponseModel()
        {
            Data = response,
        });
    }
}