using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.WareHouses;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.WarehouseModel;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WarehousesController : ControllerBase
{
    private IWarehouseService _warehouseService;

    public WarehousesController(
        IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DepartmentRequest param)
    {
        return Ok(await _warehouseService.GetAll(param));
    }


    [HttpGet("list")]
    public IActionResult GetSelectList()
    {
        var warehouses = _warehouseService.GetAll();
        return Ok(new BaseResponseModel
        {
            Data = warehouses,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var model = await _warehouseService.GetById(id);
        return Ok(model);
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromHeader] int yearFilter, [FromBody] WarehouseSetterModel warehouse)
    {
        // map model to entity and set id
        try
        {
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            }
           await  _warehouseService.Create(warehouse, userId, yearFilter);
            return Ok(new { code = 200 });
        }
        catch (ErrorException ex)
        {
            return Ok(new { code = 400, msg = ex.Message });
        }
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromHeader] int yearFilter, [FromRoute] int id, [FromBody] WarehouseSetterModel warehouse)
    {
        // map model to entity and set id
        try
        {

            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                userId = int.Parse(identity.FindFirst(x => x.Type == "UserId").Value);
            }

           await  _warehouseService.Update(warehouse, userId, yearFilter);
            return Ok(new { code = 200 });
        }
        catch (ErrorException ex)
        {
            return Ok(new { code = 400, msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _warehouseService.Delete(id);
            return Ok(new { code = 200 });
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }
}
