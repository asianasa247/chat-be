using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.ContractTypeModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Services.Interfaces.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ContractTypesController : ControllerBase
{
    private readonly IContractTypeService _contractTypeService;

    public ContractTypesController(
        IContractTypeService contractTypeService
        )
    {
        _contractTypeService = contractTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ContractTypePagingRequestModel param)
    {
        return Ok(await _contractTypeService.GetAll(param));
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetSelectList(TypeContractEnum typeContractTogether)
    {
        var contractTypes = await _contractTypeService.GetAll(typeContractTogether);
        return Ok(new BaseResponseModel
        {
            Data = contractTypes,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var contractType = await _contractTypeService.GetById(id);
        return Ok(contractType);
    }

    [HttpPost]
    public async Task<IActionResult> Craete([FromBody] ContractTypeModel model)
    {
        await _contractTypeService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ContractTypeModel model)
    {
        await _contractTypeService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _contractTypeService.Delete(id);
        return Ok();
    }
}