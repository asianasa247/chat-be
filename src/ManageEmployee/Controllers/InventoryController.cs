using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;
using ManageEmployee.Services.Interfaces.Inventorys;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class InventoryController : ControllerBase
{
    private IInventoryService _inventoryService;
    public InventoryController(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;
    }
    [HttpGet()]
    public IActionResult GetList([FromHeader] int yearFilter, [FromQuery] PagingRequestModel param)
    {
        var data = _inventoryService.GetListData(param, yearFilter);
        return Ok(data);
    }
    [HttpPost("update")]
    public IActionResult Update([FromBody] List<Inventory> data)
    {
       _inventoryService.Create(data);
        return Ok();
    }
    [HttpGet("get-list-inventory")]
    public IActionResult GetListInventory([FromQuery] InventoryRequestModel request)
    {
        var result = _inventoryService.GetListInventory(request);
        return Ok(new BaseResponseModel
        {
            TotalItems = result.Count(),
            Data = result.Skip(request.PageSize * (request.Page > 0 ? request.Page - 1 : request.Page))
           .Take(request.PageSize),
            PageSize = request.PageSize,
            CurrentPage = request.Page
        });
    }
    [HttpGet("get-list-date-inventory")]
    public IActionResult GetListDateInventory()
    {
        var data = _inventoryService.GetListDateInventory();
        return Ok(data);
    }
}
