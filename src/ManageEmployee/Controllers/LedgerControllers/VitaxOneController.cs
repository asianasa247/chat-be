using ManageEmployee.DataTransferObject.LedgerModels.VitaxInvoiceModels;
using ManageEmployee.HttpClients;
using ManageEmployee.Services.Interfaces.Companies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.LedgerControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class VitaxOneController : ControllerBase
    {
        private readonly IVitaxOneClient _vitaxOneClient;
        private readonly ICompanyService _companyService;
        public VitaxOneController(IVitaxOneClient vitaxOneClient, ICompanyService companyService)
        {
            _vitaxOneClient = vitaxOneClient;
            _companyService = companyService;
        }

        [HttpPost("new-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] VitaxOneLoginRequestModel form)
        {
            var company = await _companyService.GetCompany();
            await _vitaxOneClient.Login(form.password, company.MST);
            return Ok();
        }
    }
}