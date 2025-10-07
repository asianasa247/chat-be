using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Services.Interfaces.Statuses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StatusController : ControllerBase
{
    private readonly IStatusService _statusService;

    public StatusController(
        IStatusService statusService)
    {
        _statusService = statusService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetAll(StatusTypeEnum type)
    {
        var statuses = await _statusService.GetAll(type);

        return Ok(new BaseResponseModel()
        {
            Data = statuses,
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] StatusPagingRequest param)
    {
        var response = await _statusService.GetAll(param);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _statusService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Status entity)
    {
        var message = await _statusService.Create(entity);
        if (string.IsNullOrEmpty(message))
            return Ok(
            new ObjectReturn
            {
                message = message,
                status = Convert.ToInt32(ErrorEnum.SUCCESS)
            });
        return Ok(new ObjectReturn
        {
            message = message,
            status = Convert.ToInt32(ErrorEnum.BAD_REQUEST)
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] Status entity)
    {
        var message = await _statusService.Update(entity);
        if (string.IsNullOrEmpty(message))
            return Ok(
            new ObjectReturn
            {
                message = message,
                status = Convert.ToInt32(ErrorEnum.SUCCESS)
            });
        return Ok(new ObjectReturn
        {
            message = message,
            status = Convert.ToInt32(ErrorEnum.BAD_REQUEST)
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var message = await _statusService.Delete(id);
            if (string.IsNullOrEmpty(message))
                return Ok(new ObjectReturn
                {
                    message = message,
                    status = Convert.ToInt32(ErrorEnum.SUCCESS)
                });
            return Ok(new ObjectReturn
            {
                message = message,
                status = Convert.ToInt32(ErrorEnum.BAD_REQUEST)
            });
        }
        catch (Exception ex)
        {
            return Ok(new ObjectReturn
            {
                message = ex.Message,
                status = Convert.ToInt32(ErrorEnum.BAD_REQUEST)
            });
        }
    }
}