using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.LedgerModels;
using ManageEmployee.Models;
using ManageEmployee.Services.Interfaces.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.LedgerReportControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LedgerProjectReporterController : ControllerBase
    {
        private readonly ILedgerProjectReporter _ledgerProjectReporter;
        public LedgerProjectReporterController(ILedgerProjectReporter ledgerProjectReporter)
        {
            _ledgerProjectReporter = ledgerProjectReporter;
        }
        [HttpGet]
        [ProducesResponseType(typeof(Response<IEnumerable<LedgerProjectReporterModel>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ReportAsync([FromHeader] int yearFilter, DateTime fromAt, DateTime toAt, string projectCode, bool isNoiBo)
        {
            var response = await _ledgerProjectReporter.ReportAsync(fromAt, toAt, projectCode, yearFilter, isNoiBo);
            return Ok(new BaseResponseCommonModel
            {
                Data = response
            });
        }
    }
}
