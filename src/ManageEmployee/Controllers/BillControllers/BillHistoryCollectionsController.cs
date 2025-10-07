using ManageEmployee.DataTransferObject;
using ManageEmployee.Entities.BillEntities;
using ManageEmployee.Services.Interfaces.Bills;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.BillControllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BillHistoryCollectionsController : ControllerBase
{
    public readonly IBillHistoryCollectionService _billHistoryCollectionService;
    public BillHistoryCollectionsController(IBillHistoryCollectionService billHistoryCollectionService)
    {
        _billHistoryCollectionService = billHistoryCollectionService;
    }
    [HttpGet]
    public async Task<IActionResult> GetBillHistoryCollectionForBill(int billId)
    {
        var response = await _billHistoryCollectionService.GetBillHistoryCollectionForBill(billId);
        return Ok(new ObjectReturn
        {
            data = response,
            status = 200,
        });
    }
    [HttpPost]
    public async Task<IActionResult> Create(BillHistoryCollection request)
    {
        var response = await _billHistoryCollectionService.Create(request);
        return Ok(new ObjectReturn
        {
            data = response,
            status = 200,
        });
    }
    [HttpPut]
    public async Task<IActionResult> Update(BillHistoryCollection request)
    {
        var response = await _billHistoryCollectionService.Update(request);
        return Ok(new ObjectReturn
        {
            data = response,
            status = 200,
        });
    }
}
