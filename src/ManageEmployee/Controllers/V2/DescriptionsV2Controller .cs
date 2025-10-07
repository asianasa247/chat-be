using ManageEmployee.DataTransferObject;
using ManageEmployee.Services.Interfaces.Ledgers.V2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.V2;

[ApiController]
[Authorize]
[Route("api/v2/description")]
public class DescriptionsV2Controller : ControllerBase
{
    private readonly IDescriptionV2Service _descriptionService;

    public DescriptionsV2Controller(IDescriptionV2Service descriptionService) 
    {
        _descriptionService = descriptionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetListData([FromHeader] int yearFilter, [FromQuery] string documentCode)
    {
        var data = await _descriptionService.GetPage(yearFilter, documentCode);
        return Ok(new ObjectReturn
        {
            data = data,
        });
    }
}