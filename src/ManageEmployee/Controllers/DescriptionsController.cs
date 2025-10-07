using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;
using ManageEmployee.Services.Interfaces.Descriptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class DescriptionsController : ControllerBase
{
    private readonly IDescriptionService _descriptionService;

    public DescriptionsController(IDescriptionService descriptionService)
    {
        _descriptionService = descriptionService;
    }

    [HttpGet]
    public async Task<IActionResult> GetPage([FromQuery] PagingRequestModel param)
    {
        var data = await _descriptionService.GetPage(param.Page, param.PageSize, param.SearchText);
        var totalItems = await _descriptionService.CountAll();
        return Ok(new BaseResponseModel()
        {
            Data = data,
            TotalItems = totalItems,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }

    [HttpGet("list")]
    public IActionResult GetSelectList()
    {
        var descriptions = _descriptionService.GetAll();

        return Ok(new BaseResponseModel
        {
            Data = descriptions,
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Description entity)
    {
        var operationResult = await _descriptionService.Create(entity);
        return Ok(operationResult);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] Description entity)
    {
        var operationResult = await _descriptionService.Update(entity);
        return Ok(operationResult);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        var operationResult = await _descriptionService.Delete(id);
        return Ok(operationResult);
    }

    [HttpPost("delete-many")]
    public IActionResult Delete(List<long> id)
    {
        var message = _descriptionService.Delete(id);
        if (string.IsNullOrEmpty(message))
            return Ok();
        return BadRequest(new { msg = message });
    }
}