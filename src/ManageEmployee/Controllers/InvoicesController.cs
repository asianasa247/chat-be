using ManageEmployee.DataTransferObject;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Invoices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ManageEmployee.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InvoicesController : ControllerBase
{
    private readonly IInvoiceCreator _invoiceCreator;
    private readonly AppSettingInvoice _appSettingInvoice;

    public InvoicesController(IInvoiceCreator invoiceCreator, IOptions<AppSettingInvoice> appSettingInvoice)
    {
        _invoiceCreator = invoiceCreator;
        _appSettingInvoice = appSettingInvoice.Value;
    }

    [HttpPost]
    public async Task<IActionResult> GetAll()
    {
        await _invoiceCreator.PerformAsync(0);
        return Ok();
    }

    [HttpGet("end-point")]
    public IActionResult GetEndPoint()
    {
        return Ok(new ObjectReturn
        {
            data = _appSettingInvoice.Url,
            status = 200,
        });
    }
}