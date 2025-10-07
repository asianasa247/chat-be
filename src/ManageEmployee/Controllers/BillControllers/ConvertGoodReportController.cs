using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.BillReportModels;
using ManageEmployee.DataTransferObject.BillReportModels.ConvertGoodReports;
using ManageEmployee.Models;
using ManageEmployee.Services.Interfaces.Bills.BillReports.ConvertGoodReports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.BillControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConvertGoodReportController : ControllerBase
    {
        private readonly IGoodConvertReporter _goodConvertReporter;
        private readonly IGoodOppositeReporter _goodOppositeReporter;
        private readonly IGoodNormalReporter _goodNormalReporter;

        public ConvertGoodReportController(IGoodConvertReporter goodConvertReporter,
            IGoodOppositeReporter goodOppositeReporter,
            IGoodNormalReporter goodNormalReporter)
        {
            _goodConvertReporter = goodConvertReporter;
            _goodOppositeReporter = goodOppositeReporter;
            _goodNormalReporter = goodNormalReporter;
        }

        [ProducesResponseType(typeof(Response<IEnumerable<GoodConvertBeforeReporterModel>>), StatusCodes.Status200OK)]
        [HttpGet("good-convert")]
        public async Task<IActionResult> ReportGoodConvertReportAsync([FromHeader] int yearFilter, [FromQuery] BillReportGoodRequestModel param)
        {
            var response = await _goodConvertReporter.ReporterAsync(param, yearFilter);
            return Ok(new BaseResponseCommonModel
            {
                Data = response
            });
        }

        [ProducesResponseType(typeof(Response<IEnumerable<GoodConvertReporterModel>>), StatusCodes.Status200OK)]
        [HttpGet("good-opposite")]
        public async Task<IActionResult> ReportGoodOppositeAsync([FromHeader] int yearFilter, [FromQuery] BillReportGoodRequestModel param)
        {
            var response = await _goodOppositeReporter.ReporterAsync(param, yearFilter);
            return Ok(new BaseResponseCommonModel
            {
                Data = response
            });
        }

        [ProducesResponseType(typeof(Response<IEnumerable<GoodNormalReporterModel>>), StatusCodes.Status200OK)]
        [HttpGet("good-normal")]
        public async Task<IActionResult> ReportGoodNormalAsync([FromHeader] int yearFilter, [FromQuery] BillReportGoodRequestModel param)
        {
            var response = await _goodNormalReporter.ReporterAsync(param, yearFilter);
            return Ok(new BaseResponseCommonModel
            {
                Data = response
            });
        }
    }
}