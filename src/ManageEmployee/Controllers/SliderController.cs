using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Sliders;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SliderController : ControllerBase
{
    private readonly ISliderService _sliderService;
    private readonly IMapper _mapper;

    public SliderController(
        ISliderService sliderService,
        IMapper mapper)
    {
        _sliderService = sliderService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] SlideRequestModel param)
    {
        var result = await _sliderService.GetAll(param);
        return Ok(result);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetSlider()
    {
        var results = await _sliderService.GetAll();
        return Ok(new BaseResponseModel
        {
            Data = results,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var ward = await _sliderService.GetById(id);
        var model = _mapper.Map<SliderModel>(ward);
        return Ok(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SliderModel model)
    {
        try
        {
            model.CreateAt = DateTime.Now;
            var result = await _sliderService.Create(model);
            if (string.IsNullOrEmpty(result))
                return Ok();
            return BadRequest(new { msg = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] SliderModel model)
    {
        try
        {
            model.CreateAt = DateTime.Now;
            var result = await _sliderService.Update(model);
            if (string.IsNullOrEmpty(result))
                return Ok();
            return BadRequest(new { msg = result });
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _sliderService.Delete(id);
        return Ok();
    }
}
