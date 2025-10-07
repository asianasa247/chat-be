using ManageEmployee.DataTransferObject.EventModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.Events;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventWithImagesController : ControllerBase
{
    private readonly IEventWithImageService _eventWithImageService;

    public EventWithImagesController(IEventWithImageService eventWithImageService)
    {
        _eventWithImageService = eventWithImageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var result = await _eventWithImageService.GetAll(param);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _eventWithImageService.GetById(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] EventWithImageModel model)
    {
        await _eventWithImageService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromForm] EventWithImageModel model)
    {
        await _eventWithImageService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _eventWithImageService.Delete(id);
        return Ok();
    }
}