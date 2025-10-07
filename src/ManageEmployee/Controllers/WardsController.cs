using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Addresses;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.AddressEntities;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WardsController : ControllerBase
{
    private readonly IWardService _WardService;
    private readonly IMapper _mapper;

    public WardsController(
        IWardService WardService,
        IMapper mapper)
    {
        _WardService = WardService;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] PagingRequestModel param)
    {
        var Wards = _WardService.GetAll(param.Page, param.PageSize).ToList();
        var totalItems = _WardService.Count();
        var model = _mapper.Map<IList<Ward>>(Wards);
        return Ok(new BaseResponseModel
        {
            TotalItems = totalItems,
            Data = model,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList()
    {
        var Wards = await _WardService.GetAll(x => true);
        return Ok(new BaseResponseModel
        {
            Data = Wards,
            PageSize = 0,
            CurrentPage = 0
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var model = _WardService.GetById(id);
        return Ok(model);
    }

    [HttpGet("list/district/{districtId}")]
    public IActionResult GetListByProvinceId(int districtId)
    {
        var wards = _WardService.GetAllByDistrictId(districtId).ToList();
        var totalItems = _WardService.Count();
        var model = _mapper.Map<IList<Ward>>(wards);
        return Ok(new BaseResponseModel
        {
            TotalItems = totalItems,
            Data = model,
            PageSize = 0,
            CurrentPage = 0
        });
    }

    [HttpPost]
    public IActionResult Create([FromBody] Ward model)
    {
        // map model to entity and set id
        var Ward = _mapper.Map<Ward>(model);

        try
        {
            // update user 
            _WardService.Create(Ward);
            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Ward Ward)
    {
        try
        {
     
            // update user 
            _WardService.Update(Ward);
            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _WardService.Delete(id);
        return Ok();
    }
}
