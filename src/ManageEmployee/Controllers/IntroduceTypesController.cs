using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.IntroduceEntities;
using ManageEmployee.Services.Interfaces.Introduces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class IntroduceTypesController : ControllerBase
{
    private readonly IIntroduceTypeService _introduceTypeService;

    public IntroduceTypesController(IIntroduceTypeService introduceTypeService)
    {
        _introduceTypeService = introduceTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var result = await _introduceTypeService.GetPaging(param);
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll()
    {
        var result = await _introduceTypeService.GetList();
        return Ok(new BaseResponseModel
        {
            Data = result,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _introduceTypeService.GetById(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] IntroduceTypeModel model)
    {
        await _introduceTypeService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromForm] IntroduceType model)
    {
        await _introduceTypeService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _introduceTypeService.Delete(id);
        return Ok();
    }
}