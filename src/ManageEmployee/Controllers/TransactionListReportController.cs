using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

/// <summary>
/// Bảng kê chứng từ ghi sổ
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TransactionListReportController : Controller
{
    private readonly IVoucherService _voucherService;
    private readonly ICompanyService _companyService;

    public TransactionListReportController(
        IVoucherService voucherService,
        ICompanyService companyService)
    {
        _voucherService = voucherService;
        _companyService = companyService;
    }


    [HttpPost]
    public async Task<IActionResult> Index([FromHeader] int yearFilter, TransactionListParam param)
    {
        var from = new DateTime(DateTime.Now.Year, 1, 1);
        var to = new DateTime(DateTime.Now.Year, 12, 31, 23, 59, 59);

        if (param.FromDate.HasValue && param.ToDate.HasValue)
        {
            if (param.FromDate.Value > param.ToDate.Value)
            {
                return Ok(new BaseResponseModel()
                {
                    Data = new
                    {
                        Error = "Từ ngày phải nhỏ hơn hoặc bằng tới ngày !"
                    }
                });
            }
            from = param.FromDate.Value;
            to = param.ToDate.Value;
        }
        else
        {
            if (param.FromMonth.HasValue && param.ToMonth.HasValue)
            {
                int f = param.FromMonth.Value <= 0 ? 1 : param.FromMonth.Value;
                if (f > 12) f = 12;

                int t = param.ToMonth.Value > 12 ? 12 : param.ToMonth.Value;
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

                from = new DateTime(DateTime.Now.Year, f, 1);
                var lastDate = DateTime.DaysInMonth(DateTime.Now.Year, t);
                to = new DateTime(DateTime.Now.Year, t, lastDate, 23, 59, 59);
            }
        }

        List<VoucherInforItem> transactionList = await _voucherService.GenerateTransactionList(from, to, yearFilter, param.VoucherType, false, param.InvoiceNumber, param.InvoiceTaxCode);

        var company = await _companyService.GetCompany();

        var model = new TransactionListViewModel()
        {
            Company = company.Name,
            Address = company.Address,
            TaxId = company.MST,
            Type = param.VoucherType,
            TypeName = param.VoucherType,
            VoucherInfors = transactionList,
            ChiefAccountantName = company.NameOfChiefAccountant,
            VoteMaker = param.VoteMaker
        };

        return Ok(new BaseResponseModel()
        {
            Data = model,
        });
    }

    [HttpPost]
    [Route("get-report-transaction")]
    public async Task<IActionResult> TransactionReportList([FromHeader] int yearFilter, [FromBody] TransactionListParam param, bool isNoiBo = false)
    {
        var from = new DateTime(DateTime.Now.Year, 1, 1);
        var to = new DateTime(DateTime.Now.Year, 12, 31, 23, 59, 59);
        param.IsNoiBo = isNoiBo;
        if (param.FilterType == 2)
        {
            if (param.FromDate.Value > param.ToDate.Value)
            {
                return Ok(new BaseResponseModel()
                {
                    Data = new
                    {
                        Error = "Từ ngày phải nhỏ hơn hoặc bằng tới ngày !"
                    }
                });
            }
            from = param.FromDate.Value;
            to = param.ToDate.Value;
        }
        else
        {
            if (param.FromMonth.HasValue && param.ToMonth.HasValue)
            {
                int f = param.FromMonth.Value <= 0 ? 1 : param.FromMonth.Value;
                if (f > 12) f = 12;

                int t = param.ToMonth.Value > 12 ? 12 : param.ToMonth.Value;
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

                from = new DateTime(DateTime.Now.Year, f, 1);
                to = new DateTime(DateTime.Now.Year, t , 1).AddMonths(1);
            }
        }

        List<VoucherInforItem> transactionList = await _voucherService.GenerateTransactionList(from, to, yearFilter, param.VoucherType, isNoiBo, param.InvoiceNumber, param.InvoiceTaxCode);

        var company = await _companyService.GetCompany();

        var model = new TransactionListViewModel()
        {
            Company = company.Name,
            Address = company.Address,
            TaxId = company.MST,
            Type = param.VoucherType,
            TypeName = param.VoucherType,
            VoucherInfors = transactionList,
            ChiefAccountantName = company.NameOfChiefAccountant,
            ChiefAccountantNote = company.NoteOfChiefAccountant,
            isFillName = param.isCheckName,
            VoteMaker = param.VoteMaker
        };

        string _value = _voucherService.ExportDataTransactionList(model, param);
        if (string.IsNullOrEmpty(_value))
            return BadRequest();

        return Ok(new BaseResponseModel()
        {
            Data = _value,
        });
    }


    [HttpPost]
    [Route("get-list-taxcode")]
    public IActionResult GetListInvoiceTaxCode([FromHeader] int yearFilter, [FromBody] TransactionInvoiceParam param)
    {
        var from = new DateTime(DateTime.Now.Year, 1, 1);
        var to = new DateTime(DateTime.Now.Year, 12, 31, 23, 59, 59);

        if (param.FromDate.HasValue && param.ToDate.HasValue)
        {
            if (param.FromDate.Value > param.ToDate.Value)
            {
                return Ok(new BaseResponseModel()
                {
                    Data = new
                    {
                        Error = "Từ ngày phải nhỏ hơn hoặc bằng tới ngày !"
                    }
                });
            }
            from = param.FromDate.Value;
            to = param.ToDate.Value;
        }
        else
        {
            if (param.FromMonth.HasValue && param.ToMonth.HasValue)
            {
                int f = param.FromMonth.Value <= 0 ? 1 : param.FromMonth.Value;
                if (f > 12) f = 12;

                int t = param.ToMonth.Value > 12 ? 12 : param.ToMonth.Value;
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

                from = new DateTime(DateTime.Now.Year, f, 1);
                var lastDate = DateTime.DaysInMonth(DateTime.Now.Year, t);
                to = new DateTime(DateTime.Now.Year, t, lastDate, 23, 59, 59);
            }
        }

        var data = _voucherService.GetListInvoiceTaxCode(from, to, yearFilter, param.VoucherType);

        return Ok(new BaseResponseModel()
        {
            Data = data,
        });
    }
    [HttpPost]
    [Route("get-list-invoice-number")]
    public IActionResult GetListInvoiceNumber([FromHeader] int yearFilter, [FromBody] TransactionInvoiceParam param)
    {
        var from = new DateTime(DateTime.Now.Year, 1, 1);
        var to = new DateTime(DateTime.Now.Year, 12, 31, 23, 59, 59);

        if (param.FromDate.HasValue && param.ToDate.HasValue)
        {
            if (param.FromDate.Value > param.ToDate.Value)
            {
                return Ok(new BaseResponseModel()
                {
                    Data = new
                    {
                        Error = "Từ ngày phải nhỏ hơn hoặc bằng tới ngày !"
                    }
                });
            }
            from = param.FromDate.Value;
            to = param.ToDate.Value;
        }
        else
        {
            if (param.FromMonth.HasValue && param.ToMonth.HasValue)
            {
                int f = param.FromMonth.Value <= 0 ? 1 : param.FromMonth.Value;
                if (f > 12) f = 12;

                int t = param.ToMonth.Value > 12 ? 12 : param.ToMonth.Value;
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

                from = new DateTime(DateTime.Now.Year, f, 1);
                var lastDate = DateTime.DaysInMonth(DateTime.Now.Year, t);
                to = new DateTime(DateTime.Now.Year, t, lastDate, 23, 59, 59);
            }
        }

        var data = _voucherService.GetListInvoiceNumber(from, to, yearFilter, param.VoucherType);

        return Ok(new BaseResponseModel()
        {
            Data = data,
        });
    }

}