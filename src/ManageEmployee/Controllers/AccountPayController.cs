using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.Entities.UserEntites;
using ManageEmployee.Services.Interfaces.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class AccountPayController : ControllerBase
{
    private readonly IAccountPayService _account;
    public AccountPayController(IAccountPayService account)
    {
        _account = account;
    }
    [Route("GetAll")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _account.GetAll();
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }
    [HttpPut("update")]
    public async Task<IActionResult> Update([FromBody] AccountPay model)
    {
        var result = await _account.UpdateAccountPay(model);
        return Ok(new BaseResponseModel { Data = result });
    }
}
