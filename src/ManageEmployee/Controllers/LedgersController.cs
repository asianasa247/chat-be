using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.LedgerModels.VitaxInvoiceModels;
using ManageEmployee.DataTransferObject.LedgerWarehouseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PagingResultModels;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.LedgerEntities;
using ManageEmployee.Entities.LedgerEntities.VitaxEntities;
using ManageEmployee.Extends;
using ManageEmployee.Services.Interfaces.Invoices;
using ManageEmployee.Services.Interfaces.Ledgers;
using ManageEmployee.Services.Interfaces.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LedgersController : ControllerBase
{
    private readonly ILedgerService _ledgerService;
    private readonly IFixedAssetsService _fixedAssetsService;
    private readonly ApplicationDbContext _context;
    private readonly IReportDebitCustomerService _reportDebitCustomerService;
    private readonly ILedgerDetailBookService _ledgerDetailBookService;
    private readonly ILedgerDangKiChungTuGhiSoService _ledgerDangKiChungTuGhiSoService;
    private readonly IInvoiceInGetter _invoiceInGetter;
    private readonly ISoChiTiet_FourReporter _soChiTiet_FourReporter;

    public LedgersController(ILedgerService ledgerService, IFixedAssetsService fixedAssetsService
        , ApplicationDbContext context, IReportDebitCustomerService reportDebitCustomerService
        , ILedgerDetailBookService ledgerDetailBookService, ILedgerDangKiChungTuGhiSoService ledgerDangKiChungTuGhiSoService,
        IInvoiceInGetter invoiceInGetter, ISoChiTiet_FourReporter oChiTiet_FourReporter)
    {
        _ledgerService = ledgerService;
        _fixedAssetsService = fixedAssetsService;
        _context = context;
        _reportDebitCustomerService = reportDebitCustomerService;
        _ledgerDetailBookService = ledgerDetailBookService;
        _ledgerDangKiChungTuGhiSoService = ledgerDangKiChungTuGhiSoService;
        _invoiceInGetter = invoiceInGetter;
        _soChiTiet_FourReporter = oChiTiet_FourReporter;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Ledger entity, [FromHeader] int yearFilter)
    {
        Ledger operationResult;
        entity.Year = yearFilter;

        var currentLedger = await _ledgerService.GetLedgerById(entity.Id, entity.IsInternal);

        var userId = HttpContext.GetIdentityUser().Id;
        
        if (currentLedger != null)
        {
            if (entity.Type == "PC" && entity.ReferenceVoucherNumber != currentLedger.ReferenceVoucherNumber)
            {
                var listLedger = await _context.GetLedger(yearFilter, entity.IsInternal).Where(x => x.OrginalVoucherNumber == entity.OrginalVoucherNumber).ToListAsync();
                listLedger.ForEach(ele =>
                {
                    ele.ReferenceVoucherNumber = entity.ReferenceVoucherNumber;
                });
                _context.UpdateRange(listLedger);
            }

            currentLedger.UserUpdated = userId;

            operationResult = await _ledgerService.Update(currentLedger, entity, yearFilter);
        }
        else
        {
            entity.UserCreated = userId;
            operationResult = await _ledgerService.Create(entity, yearFilter);
            if (operationResult != null && entity.DepreciaMonth > 0)
            {
                await _fixedAssetsService.UpdateFromLedger(entity, yearFilter);
            }
        }

        return Ok(operationResult);
    }

    [HttpGet]// get-page
    public async Task<IActionResult> GetPage([FromQuery] LedgerRequestModel request, [FromHeader] int yearFilter)
    {
        var data = await _ledgerService.GetPage(request, yearFilter);

        return Ok(data);
    }

    [HttpGet("get-cost-of-goods-page")]
    public async Task<IActionResult> GetCostOfGoods([FromQuery] LedgerRequestModel request, [FromHeader] int yearFilter)
    {
        var data = await _ledgerService.GetCostOfGoods(request.FilterMonth, yearFilter, request.IsInternal, yearFilter);

        return Ok(new BaseResponseModel
        {
            Data = data, //data.Skip((request.page - 1) * request._pageSize).Take(request._pageSize),
            TotalItems = data.Count,
            PageSize = request.PageSize,
            CurrentPage = request.Page
        });
    }

    [HttpPost("create-cost-of-goods")]
    public async Task<IActionResult> CreateCostOfGoods([FromBody] List<LedgerCostOfGoodsModel> entities, [FromHeader] int yearFilter, [FromQuery] int isInternal)
    {
        var result = await _ledgerService.CreateCostOfGoods(entities, isInternal, yearFilter);
        if (string.IsNullOrEmpty(result))
            return Ok(result);
        return BadRequest(new { msg = result });
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] string ids, [FromHeader] int yearFilter, int isInternal = 1)
    {
        var message = await _ledgerService.Delete(ids, isInternal, yearFilter);
        if (string.IsNullOrEmpty(message))
            return Ok();
        return BadRequest(new { msg = message });
    }

    [HttpPost("edit-order")]
    public async Task<IActionResult> EditOrder([FromBody] EditOrderRequestModel request, [FromHeader] int yearFilter)
    {
        var message = await _ledgerService.EditOrder(request, yearFilter);
        if (string.IsNullOrEmpty(message))
            return Ok(new ObjectReturn
            {
                data = message,
                status = 200,
            });
        return BadRequest(new { msg = message });
    }

    [HttpPost]
    [Route("get-report-ledger")]
    public async Task<IActionResult> GetReportLedger([FromBody] LedgerReportParam request, [FromHeader] int yearFilter, bool isNoiBo = false)
    {
        var data = await _ledgerService.GetDataReport(request, yearFilter, isNoiBo);
        return Ok(new BaseResponseModel
        {
            Data = data
        });
    }

    [HttpPost]
    [Route("get-report-dkctgs-ledger")]
    public async Task<IActionResult> GetReportDKCTGSLedger([FromBody] LedgerReportParam request, [FromHeader] int yearFilter, bool isNoiBo = false)
    {
        var data = await _ledgerDangKiChungTuGhiSoService.GetDataReport_DangKyChungTuGhiSo(request, yearFilter, isNoiBo);
        return Ok(new BaseResponseModel
        {
            Data = data
        });
    }

    [HttpPost]
    [Route("get-report-sct-ledger")]
    public async Task<IActionResult> GetSoChiTietReportLedger([FromBody] LedgerReportParamDetail request, [FromHeader] int yearFilter, bool isNoiBo = false)
    {
        _ledgerService.ValidateDataReport(request);
        
        string data;
        request.IsNoiBo = isNoiBo;
        if (request.BookDetailType == (int)ReportBookDetailTypeEnum.bienBanCongNo)
            data = await _reportDebitCustomerService.ReportAsyn(request, yearFilter);
        else if (request.BookDetailType == (int)ReportBookDetailTypeEnum.soSoLuongTonKho)
            data = await _ledgerDetailBookService.GetDataReport_SoChiTiet_Six(request, yearFilter);
        else if (request.BookDetailType == (int)ReportBookDetailTypeEnum.soCoHangTonKho)
            data = await _soChiTiet_FourReporter.ReportAsync(request, yearFilter);
        else
            data = await _ledgerDetailBookService.GetDataReport_SoChiTiet_Full(request, yearFilter, isNoiBo);
        return Ok(new BaseResponseModel
        {
            Data = data
        });
    }

    [HttpPost]
    [Route("get-total-amount-tax")]
    public async Task<IActionResult> DinhKhoanThue([FromBody] LedgerRequestDinhKhoanThue request, [FromHeader] int yearFilter)
    {
        double data = await _ledgerService.DinhKhoanThue(request, yearFilter);
        return Ok(new BaseResponseModel
        {
            Data = data
        });
    }

    [HttpGet]
    [Route("get-list-ledger-print")]
    public async Task<IActionResult> GetListDataPrint(string? OrginalVoucherNumber, int isInternal, [FromHeader] int yearFilter)
    {
        var data = await _ledgerService.GetListDataPrint(OrginalVoucherNumber, isInternal, yearFilter);
        return Ok(new BaseResponseModel
        {
            Data = data
        });
    }


    [HttpGet]
    [Route("vitax-invoice-in")]
    [ProducesResponseType(typeof(PagingResult<VintaxInvoiceIn>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInvoiceIn([FromQuery] PagingRequestFilterDateModel param)
    {
        var response = await _invoiceInGetter.GetInvoice(param);
        return Ok(response);
    }
    [HttpGet]
    [Route("vitax-invoice-in-detail/{id}")]
    [ProducesResponseType(typeof(IEnumerable<LedgerVitaxInvoiceInModel>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetInvoiceInDetail(string id)
    {
        var response = await _invoiceInGetter.GetDetail(id);
        return Ok(response);
    }

    [HttpGet]
    [Route("xoa-xuat-kho-gia-von")]
    public async Task<IActionResult> XoaXuatKhoGiaVon()
    {
        var response = await _ledgerService.XoaXuatKhoGiaVon();
        return Ok(response);
    }
}