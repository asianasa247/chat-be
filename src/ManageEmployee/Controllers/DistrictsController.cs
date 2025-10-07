using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Addresses;
using ManageEmployee.Entities.AddressEntities;
using ManageEmployee.DataTransferObject.AddressModels;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DistrictsController : ControllerBase
{
    private readonly IDistrictService _districtService;
    private readonly IMapper _mapper;

    public DistrictsController(
        IDistrictService districtService,
        IMapper mapper)
    {
        _districtService = districtService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DistrictViewModel.GetByProvince param)
    {
        var districts = new List<District>();
        var totalItems =0;
        if (param.provinceid != null && param.provinceid != 0)
        {
            districts = _districtService.GetMany(x => !x.IsDeleted && x.ProvinceId == param.provinceid.Value, param.Page, param.PageSize).ToList();
            totalItems = _districtService.Count(x => !x.IsDeleted && x.ProvinceId == param.provinceid.Value);
        }
        else
        {
            districts = await _districtService.GetAll(param.Page, param.PageSize);
            totalItems =  _districtService.Count();
        }
     
        var model = _mapper.Map<IList<DistrictModel>>(districts);
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
        var districts = await _districtService.GetAll(x => true);
        return Ok(new BaseResponseModel
        {
            Data = districts,
            PageSize = 0,
            CurrentPage = 0
        });
    }

    [HttpGet("list/province/{provinceId}")]
    public async Task<IActionResult> GetListByProvinceId(int provinceId)
    {
        var districts = await _districtService.GetAllByProvinceId(provinceId);
        var totalItems = _districtService.Count();
        var model = _mapper.Map<IList<District>>(districts);
        return Ok(new BaseResponseModel
        {
            TotalItems = totalItems,
            Data = model,
            PageSize = 0,
            CurrentPage = 0
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var district = _districtService.GetById(id);
        var model = _mapper.Map<District>(district);
        return Ok(model);
    }

    [HttpPost]
    public IActionResult Create([FromBody] DistrictModel model)
    {
        // map model to entity and set id
        var district = _mapper.Map<District>(model);

        try
        {
            // update user 
            _districtService.Create(district);
            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] DistrictModel model)
    {
        // map model to entity and set id
        var district = _mapper.Map<District>(model);
        district.Id = id;

        try
        {
            // update user 
            _districtService.Update(district);
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
        _districtService.Delete(id);
        return Ok();
    }
}
