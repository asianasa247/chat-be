using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Prints;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.DataTransferObject.PrintViewModelModels;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PrintsController : ControllerBase
{
    private IPrintService _printService;

    public PrintsController(
        IPrintService printService)
    {
        _printService = printService;
    }

    [HttpGet]
    public IActionResult GetAll([FromQuery] PagingRequestModel param)
    {
        var data = _printService.GetAll();
        return Ok(new BaseResponseModel
        {
            TotalItems = data.Count(),
            Data = data.Skip(param.PageSize * (param.Page - 1)).ToList()
             .Take(param.PageSize),
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }

    [HttpGet("list")]
    public IActionResult GetList()
    {
        var result = _printService.GetAll().ToList();
        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }

    [HttpGet("get-page-print")]
    public IActionResult GetPagePrint()
    {
        var result = _printService.GetPagePrint();
        return Ok(new ObjectReturn
        {
            data = result,
            status = 200,
        });
    }
  
    [HttpPut]
    public IActionResult Update([FromBody] PagePrintViewModel model)
    {
        try
        {
            _printService.Update(model);
            return Ok(new ObjectReturn
            {
                data = "",
                status = 200,
            });
        }
        catch (ErrorException ex)
        {
            return Ok(new ObjectReturn
            {
                data = ex.Message.ToString(),
                status = 200,
            });
        }
    }
}
