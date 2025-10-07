using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Services.Interfaces.Jobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;

    public JobsController(
        IJobService jobService)
    {
        _jobService = jobService;
    }

    [HttpGet("list")]
    public IActionResult GetAll()
    {
        var jobs = _jobService.GetAll();
     
        return Ok(new BaseResponseModel()
        {
            Data = jobs,
        });
    }

    [HttpGet("customer-histories/{customerId}/jobs-and-statuses")]
    public async Task<IActionResult> GetJobsAndStatusesExistingCustomerHistoriesAsync(int customerId)
    {
        return Ok(await _jobService.GetJobsAndStatusesExistingCustomerHistoriesAsync(customerId));
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] PagingRequestModel param)
    {
        var jobs = _jobService.GetPaging(param.Page,param.PageSize,param.SearchText);
        var totalItems = _jobService.Count(param.SearchText);
        return Ok(new BaseResponseModel
        {
            TotalItems = totalItems,
            Data = jobs,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var model = _jobService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Job entity)
    {
        var message = await _jobService.Create(entity);
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
    public async Task<IActionResult> Update([FromBody] Job entity)
    {
        var message = await _jobService.Update(entity);
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
    public IActionResult Delete(int id)
    {
        try
        {
            _jobService.Delete(id);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
       
    }
}