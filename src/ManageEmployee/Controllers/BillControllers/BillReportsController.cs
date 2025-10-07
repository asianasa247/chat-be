using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.BillModels;
using ManageEmployee.DataTransferObject.BillReportModels;
using ManageEmployee.DataTransferObject.BillReportModels.BusinessOverviews;
using ManageEmployee.DataTransferObject.BillReportModels.GoodReports;
using ManageEmployee.DataTransferObject.BillReportModels.OperatingCosts;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Models;
using ManageEmployee.Services.Interfaces.Bills;
using ManageEmployee.Services.Interfaces.Bills.BillReports.BusinessOverviews;
using ManageEmployee.Services.Interfaces.Bills.BillReports.OperatingCosts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.BillControllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class BillReportsController : ControllerBase
{
    private readonly IBillReporter _billReporter;
    private readonly IBusinessOverviewReporter _businessOverviewReporter;
    private readonly IBusinessDetailForRevenueReporter _businessDetailForRevenueReporter;
    private readonly IBusinessDetailForRefundReporter _businessDetailForRefundReporter;
    private readonly IBusinessDetailForTotalBillReporter _businessDetailForTotalBillReporter;
    private readonly IOperatingCostOverviewReporter _operatingCostOverviewReporter;
    private readonly IBusinessOverviewChartReporter _businessOverviewChartReporter;
    private readonly IGoodOverviewReporter _goodOverviewReporter;

    public BillReportsController(IBillReporter billReporter,
        IBusinessOverviewReporter businessOverviewReporter,
        IBusinessDetailForRevenueReporter businessDetailForRevenueReporter,
        IBusinessDetailForRefundReporter businessDetailForRefundReporter,
        IBusinessDetailForTotalBillReporter businessDetailForTotalBillReporter,
        IOperatingCostOverviewReporter operatingCostOverviewReporter, 
        IBusinessOverviewChartReporter businessOverviewChartReporter, 
        IGoodOverviewReporter goodOverviewReporter)
    {
        _billReporter = billReporter;
        _businessOverviewReporter = businessOverviewReporter;
        _businessDetailForRevenueReporter = businessDetailForRevenueReporter;
        _businessDetailForRefundReporter = businessDetailForRefundReporter;
        _businessDetailForTotalBillReporter = businessDetailForTotalBillReporter;
        _operatingCostOverviewReporter = operatingCostOverviewReporter;
        _businessOverviewChartReporter = businessOverviewChartReporter;
        _goodOverviewReporter = goodOverviewReporter;
    }
    /// <summary>
    /// 
    /// </summary>
    [HttpGet("report")]
    [ProducesResponseType(typeof(Response<BillReporterModel>), StatusCodes.Status200OK)]

    public async Task<IActionResult> GetLedgerFromBillId([FromQuery] BillPagingRequestModel param)
    {
        var result = await _billReporter.ReportAsync(param);
        return Ok(new BaseResponseModel
        {
            Data = result
        });
    }

    [HttpGet("report-home")]

    public async Task<IActionResult> ReportHome([FromHeader] int yearFilter, [FromQuery] RequestFilterDateModel query)
    {
        var result = await _billReporter.ReportHomeAsync(query, yearFilter);
        return Ok(result);
    }

    /// <summary>
    /// Báo cáo tổng quan kinh doanh
    /// </summary>
    [ProducesResponseType(typeof(Response<BusinessOverviewReportModel>), StatusCodes.Status200OK)]
    [HttpGet("business-overview")]
    public async Task<IActionResult> BusinessOverviewReporter([FromHeader] int yearFilter, [FromQuery] BillReportBranchRequestModel param)
    {
        var response = await _businessOverviewReporter.ReportOverviewAsync(param, yearFilter);
        return Ok(new BaseResponseCommonModel
        {
            Data = response
        });
    }

    [ProducesResponseType(typeof(Response<IEnumerable<BusinessOverviewReportModel>>), StatusCodes.Status200OK)]
    [HttpGet("chart-business-overview")]
    public async Task<IActionResult> BusinessOverviewChartReporter([FromHeader] int yearFilter, [FromQuery] BillReportBranchRequestModel param)
    {
        var response = await _businessOverviewChartReporter.ReportAsync(param, yearFilter);
        return Ok(new BaseResponseCommonModel
        {
            Data = response
        });
    }

    /// <summary>
    /// Báo cáo tổng quan kinh doanh
    /// </summary>
    [ProducesResponseType(typeof(Response<IEnumerable<BusinessReportForBranchModel>>), StatusCodes.Status200OK)]
    [HttpGet("business-overview-for-branch")]
    public async Task<IActionResult> ReportForBranchAsync([FromHeader] int yearFilter, [FromQuery] BillReportBranchRequestModel param)
    {
        var response = await _businessOverviewReporter.ReportForBranchAsync(param, yearFilter);
        return Ok(new BaseResponseCommonModel
        {
            Data = response
        });
    }

    [ProducesResponseType(typeof(Response<BusinessDetailForGroupGoodReporterModel>), StatusCodes.Status200OK)]
    [HttpGet("business-detail-revenue")]
    public async Task<IActionResult> ReportForGroupGoodAsync([FromQuery] BillReportBranchRequestModel param)
    {
        var response = await _businessDetailForRevenueReporter.ReportAsync(param);
        return Ok(new BaseResponseCommonModel
        {
            Data = response
        });
    }

    // <summary>
    /// Báo cáo tổng quan kinh doanh
    /// </summary>
    [ProducesResponseType(typeof(Response<BusinessDetailForGroupGoodReporterModel>), StatusCodes.Status200OK)]
    [HttpGet("business-detail-refund")]
    public async Task<IActionResult> ReportRefundAmountForGroupGoodAsync([FromQuery] BillReportBranchRequestModel param)
    {
        var response = await _businessDetailForRefundReporter.ReportAsync(param);
        return Ok(new BaseResponseCommonModel
        {
            Data = response
        });
    }

    // <summary>
    /// Báo cáo tổng quan kinh doanh
    /// </summary>
    [ProducesResponseType(typeof(Response<BusinessDetailForGroupGoodReporterModel>), StatusCodes.Status200OK)]
    [HttpGet("group-total-bill")]
    public async Task<IActionResult> ReportTotalBillForGroupGoodAsync([FromQuery] BillReportBranchRequestModel param)
    {
        var response = await _businessDetailForTotalBillReporter.ReportAsync(param);
        return Ok(new BaseResponseCommonModel
        {
            Data = response
        });
    }

    [ProducesResponseType(typeof(Response<OperatingCostOverviewReportModel>), StatusCodes.Status200OK)]
    [HttpGet("operating-cost")]
    public async Task<IActionResult> OperatingCostOverviewReporterAsync([FromHeader] int yearFilter, [FromQuery] BillReportBranchRequestModel param)
    {
        var response = await _operatingCostOverviewReporter.ReportAsync(param, yearFilter);
        return Ok(new BaseResponseCommonModel
        {
            Data = response
        });
    }

    [ProducesResponseType(typeof(Response<GoodOverviewReporterModel>), StatusCodes.Status200OK)]
    [HttpGet("good-seller")]
    public async Task<IActionResult> GoodOverviewReporterAsync([FromHeader] int yearFilter, [FromQuery] BillReportBranchRequestModel param)
    {
        var response = await _goodOverviewReporter.ReportAsync(param, yearFilter);
        return Ok(new BaseResponseCommonModel
        {
            Data = response
        });
    }
}