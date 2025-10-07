using ManageEmployee.DataTransferObject.Web;
using ManageEmployee.Services.Interfaces.Addresses;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.Web;

[Route("api/[controller]")]
[ApiController]
public class WebBranchController : ControllerBase
{
    private readonly IBranchService _branchService;

    public WebBranchController(
        IBranchService branchService)
    {
        _branchService = branchService;
    }

    [HttpGet("getAll")]
    public async Task<IActionResult> GetBranch()
    {
        var results = await _branchService.GetAll();

        return Ok(new CommonWebResponse()
        {
            State = true,
            Code = 200,
            Message = "",
            Data = results
        });
    }
}