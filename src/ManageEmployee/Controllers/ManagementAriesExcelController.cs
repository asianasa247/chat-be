using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.AriseModels;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces;
using ManageEmployee.Services.Interfaces.ChartOfAccounts;
using ManageEmployee.Services.Interfaces.Excels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class ManagementAriesExcelController : ControllerBase
{
    private readonly IManagementAriesExcelService _managementAriesExcelService;
    private readonly IChartOfAccountService _chartOfAccountService;
    private readonly IConnectionStringProvider _connectionStringProvider;
    public ManagementAriesExcelController(
        IManagementAriesExcelService managementAriesExcelService,
        IChartOfAccountService chartOfAccountService,
        IConnectionStringProvider connectionStringProvider)
    {
        _managementAriesExcelService = managementAriesExcelService;
        _chartOfAccountService = chartOfAccountService;
        _connectionStringProvider = connectionStringProvider;
    }
    /// <summary>
    /// Lấy Ledger cuối cùng của tháng
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("getLastVoucherNumberInMonth")]
    public IActionResult getLastVoucherNumberInMonth([FromBody] AriesExcelSearchModel request, [FromHeader] int yearFilter)
    {
        var result = _managementAriesExcelService.GetLastVoucherNumberInMonth(request, yearFilter);

        return Ok(result);
    }

    /// <summary>
    /// Lấy Ledger cuối cùng của tháng
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("exportAries")]
    public async Task<IActionResult> ExportAries([FromBody] AriesExcelSearchModel request, [FromHeader] int yearFilter)
    {
        var stream = await _managementAriesExcelService.ExportExcel(request, yearFilter);
        string fileName = string.Format("SoKeToan_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmm"));
        return this.File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
    [HttpPost("exportAries-sample")]
    public IActionResult ExportAriesSample()
    {

        var stream = _managementAriesExcelService.ExportExcelSample();
        string fileName = string.Format("SoKeToanMau_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmm"));
        return this.File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    /// <summary>
    /// Import file excel
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("importExcel")]
    public async Task<IActionResult> ImportAries([FromBody] AriseExcelImportModel request, [FromHeader] int yearFilter)
    {
        var dbName = _connectionStringProvider.GetConnectionString();

        await _managementAriesExcelService.ImportExcel(request.Ledgers, yearFilter);
        await _chartOfAccountService.UpdateArisingAccount(yearFilter, dbName);
        return Ok(new ObjectReturn
        {
            status = 200
        });
    }

    /// <summary>
    /// Cập nhật OrginalVoucher theo khoảng tháng
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("updateOrginalVoucherNumber")]
    public async Task<IActionResult> updateOrginalVoucherNumber([FromBody] AriseUpdateOrginalVoucherRequest request, [FromHeader] int yearFilter)
    {
        await _managementAriesExcelService.UpdateOrginalVoucherNumber(request, yearFilter);
        return Ok(new ObjectReturn
        {
            status = 200,
            data = ""
        });
    }

    /// <summary>
    /// Thay đổi loại chứng từ/tháng
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("transferInfoLedger")]
    public async Task<IActionResult> TransferInfoLedger([FromBody] TransferModelRequest request, [FromHeader] int yearFilter)
    {
        if (request.LedgerIds == null || request.LedgerIds.Count == 0)
            return Ok(new ObjectReturn
            {
                status = 400,
                data = "Bạn chưa chọn chứng từ"
            });
        if (request.Month == 0)
            return Ok(new ObjectReturn
            {
                status = 400,
                data = "Bạn chưa chọn tháng chuyển đổi"
            });
        if (string.IsNullOrEmpty(request.DocumentType))
            return Ok(new ObjectReturn
            {
                status = 400,
                data = "Bạn chưa chọn loại chứng từ chuyển đổi"
            });
        await _managementAriesExcelService.TransferInfoLedger(request, yearFilter);

        return Ok(new ObjectReturn
        {
            status = 200,
            data = ""
        });
    }

    /// <summary>
    /// Thay đổi loại chứng từ/tháng
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpGet("getDebitAndCreditAccount")]
    public async Task<IActionResult> GetDebitAndCreditAccount([FromHeader] int yearFilter)
    {
        var result = await _managementAriesExcelService.GetDebitAndCreditAccount(yearFilter);
        return Ok(new BaseResponseModel
        {
            CurrentPage = 1,
            DataTotal = 0,
            Data = result
        });
    }

    [HttpPost("TransferInfoLedgerLuong")]
    public async Task<IActionResult> TransferInfoLedgerLuong([FromBody] TransferModelRequest request, [FromHeader] int yearFilter)
    {
        await _managementAriesExcelService.TransferInfoLedgerLuong(request, yearFilter);
        return Ok(new ObjectReturn
        {
            status = 200,
            data = ""
        });
    }

}
