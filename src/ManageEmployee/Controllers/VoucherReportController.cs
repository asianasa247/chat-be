using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Helpers;
using ManageEmployee.Services.Interfaces.Reports;
using ManageEmployee.Services.Interfaces.Companies;
using ManageEmployee.Services.Interfaces.Documents;
using ManageEmployee.DataTransferObject.Reports;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class VoucherReportController : Controller
{
    private readonly IVoucherService _voucherService;
    private readonly ICompanyService _companyService;
    private readonly IDocumentService _documentService;

    public VoucherReportController(
        IVoucherService voucherService,
        ICompanyService companyService,
        IDocumentService documentService
        )
    {
        _voucherService = voucherService;
        _companyService = companyService;
        _documentService = documentService;
    }


    [HttpPost]
    public async Task<IActionResult> Index(VoucherReportParam param)
    {
        var timeRange = TimeRangeFromRequest.TimeRange(param.FromDate, param.ToDate, param.FromMonth, param.ToMonth);

        List<VoucherReportItem> voucherReportItems = await _voucherService.GenerateVoucherReportV2(timeRange.From, timeRange.To,"","", param.VoucherType);

        var company = await _companyService.GetCompany();
        var documentTypeName = _documentService.GetDocumentTypeName(param.VoucherType);

        var model = new VoucherReportViewModel()
        {
            Company = company.Name,
            Address = company.Address,
            TaxId = company.MST,
            Type = param.VoucherType,
            TypeName = documentTypeName,
            Items = voucherReportItems,
            ChiefAccountantName = company.NameOfChiefAccountant,
            VoteMaker = param.VoteMaker
        };

        return Ok(new BaseResponseModel()
        {
            Data = model,
        });
    }

    [HttpPost]
    [Route("get-report-voucher")]
    public async Task<IActionResult> ExportData(VoucherReportParam param)
    {
        var timeRange = TimeRangeFromRequest.TimeRange(param.FromDate, param.ToDate, param.FromMonth, param.ToMonth);

        List<VoucherReportItem> voucherReportItems = await _voucherService.GenerateVoucherReportV2(timeRange.From, timeRange.To,param.InvoiceTaxCode, param.InvoiceNumber, param.VoucherType, param.IsNoiBo);

        var company = await _companyService.GetCompany();

        var model = new VoucherReportViewModel()
        {
            Company = company.Name,
            Address = company.Address,
            TaxId = company.MST,
            Type = param.VoucherType,
            TypeName = param.VoucherType,
            Items = voucherReportItems,
            ChiefAccountantName = company.NameOfChiefAccountant,
            VoteMaker = param.VoteMaker,
            NoteChiefAccountantName = company.NoteOfChiefAccountant
        };

        string _value = _voucherService.ExportDataVoucher(model, param);
        if (string.IsNullOrEmpty(_value))
            return BadRequest();

        return Ok(new BaseResponseModel()
        {
            Data = _value,
        });
    }

}
