using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.Entities;
using ManageEmployee.Services.Interfaces.DeskFloors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DeskFloorsController : ControllerBase
{
    private readonly IDeskFloorService _deskFloorService;

    public DeskFloorsController(IDeskFloorService deskFloorService)
    {
        _deskFloorService = deskFloorService;
    }

    [HttpGet()]
    public async Task<IActionResult> GetAll([FromQuery] DeskFLoorPagingationRequestModel param)
    {
        var result = await _deskFloorService.GetPaging(param);
        return Ok(new BaseResponseModel
        {
            TotalItems = result.TotalItems,
            Data = result.DeskFloors,
            PageSize = result.PageSize,
            CurrentPage = result.pageIndex
        });
    }

    [HttpGet("getdeskfloor")]
    public async Task<IActionResult> GetDeskFloor()
    {
        var deskFloors = await _deskFloorService.GetAll();

        return Ok(new BaseResponseModel
        {
            Data = deskFloors,
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var res = await _deskFloorService.GetById(id);
        return Ok(res);
    }

    [HttpGet("by-code/{code}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByCode(string? code)
    {
        var res = await _deskFloorService.GetByCode(code);
        return Ok(res);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] DeskFloor model)
    {
        await _deskFloorService.Create(model);
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromBody] DeskFloor model, int id)
    {
        await _deskFloorService.Update(model);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _deskFloorService.Delete(id);
        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("get-list-desk-free")]
    public async Task<IActionResult> GetListDeskFree()
    {
        var result = await _deskFloorService.GetListDeskFree();

        return Ok(
               new ObjectReturn
               {
                   data = result,
                   message = "success",
                   status = 200
               });
    }

    [HttpPut("reset-desk")]
    public async Task<IActionResult> ResetDeskChoose()
    {
        await _deskFloorService.ResetDeskChoose();
        return Ok();
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus([FromRoute] int id,[FromQuery] int statusId)
    {
        await _deskFloorService.UpdateStatus(id, statusId);
        return Ok();
    }
}