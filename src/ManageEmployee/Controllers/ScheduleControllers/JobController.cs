using Hangfire;
using ManageEmployee.DataTransferObject.LedgerModels;
using ManageEmployee.JobSchedules.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.ScheduleControllers
{
    [ApiController]
    [Route("api/schedule")]

    public class JobController : ControllerBase
    {
        private readonly IVitaxInvoiceGetterJob _vitaxInvoiceGetterJob;

        public JobController(IVitaxInvoiceGetterJob vitaxInvoiceGetterJob)
        {
            _vitaxInvoiceGetterJob = vitaxInvoiceGetterJob;
        }

        [HttpPost("vitax-invoice-job")]
        public async Task<IActionResult> GetVitaxInvoice([FromBody] RequestSyncInvoice request)
        {
            var result = await _vitaxInvoiceGetterJob.GetInvoice(request.FromDate, request.ToDate);

            if (result.success)
            {
                return Ok(new
                {
                    success = true,
                    message = result.msg
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    message = result.msg
                });
            }
        }
    }
}
