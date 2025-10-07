using AutoMapper;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.Entities;
using ManageEmployee.Services.Interfaces.Relatives;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class RelationShipsController : ControllerBase
{
    private readonly IRelationShipService _relationShipService;
    private readonly IMapper _mapper;
    public RelationShipsController(IRelationShipService relationShipService, IMapper mapper)
    {
        _relationShipService = relationShipService;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetById([FromQuery] RelationShipViewModel param)
    {
        if (param.Page == 0)
            param.Page = 1;
        var model = _relationShipService.GetListPaging(param.PageSize,param.Page,param.EmployeeId);
        var totalItems = _relationShipService.Count(param.EmployeeId);

        return Ok(new BaseResponseModel
        {
            TotalItems = totalItems,
            Data = model,
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var user = _relationShipService.GetById(id);
        return Ok(user);
    }
    [HttpPost]
    public IActionResult AddNew([FromBody]RelationShip relation)
    {
        var result = _relationShipService.Create(relation);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public IActionResult Update([FromBody] RelationShip relation)
    {
        var result = _relationShipService.Update(relation);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public IActionResult Deleted(int id)
    {
        _relationShipService.Delete(id);
        return Ok(true);
    }
}
