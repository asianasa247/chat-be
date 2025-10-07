using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.ContractTypeModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]

public class ContractFilesController : ControllerBase
{
    private readonly IContractFileService _contractFileService;

    public ContractFilesController(IContractFileService contractFileService)
    {
        _contractFileService = contractFileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        return Ok(await _contractFileService.GetAll(param));
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetSelectList(int contractTypeId)
    {
        var contractTypes = await _contractFileService.GetAll(contractTypeId);
        return Ok(new BaseResponseModel
        {
            Data = contractTypes,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var contractType = await _contractFileService.GetById(id);
        return Ok(contractType);
    }

    [HttpPost]
    public async Task<IActionResult> Craete([FromBody] ContractFileModel model)
    {
        await _contractFileService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ContractFileModel model)
    {
        await _contractFileService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _contractFileService.Delete(id);
        return Ok();
    }
}
