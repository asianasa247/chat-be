using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Majors;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class MajorsController : ControllerBase
{
    private IMajorService _majorService;
    private IMapper _mapper;
    private readonly AppSettings _appSettings;

    public MajorsController(
        IMajorService majorService,
        IMapper mapper,
        IOptions<AppSettings> appSettings)
    {
        _majorService = majorService;
        _mapper = mapper;
        _appSettings = appSettings.Value;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        return Ok(await _majorService.GetAll(param.Page, param.PageSize, param.SearchText));
    }


    [HttpGet("list")]
    public async Task<IActionResult> GetSelectList()
    {
        var majors = await _majorService.GetAll();

        return Ok(new BaseResponseModel
        {
            Data = majors,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _majorService.GetById(id);
        return Ok(model);
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Major major)
    {
        // map model to entity and set id

        try
        {
            await _majorService.Create(major);

            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return Ok(new { code = 400, msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] Major major)
    {
        try
        {
            await _majorService.Update(major);

            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return Ok(new { code = 400, msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await  _majorService.Delete(id);
        return Ok();
    }
}
