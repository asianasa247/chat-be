using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Orders;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.OrderEntities;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrderSuccessfulController : ControllerBase
{
    private IOrderSuccessfulService _ordersuccessful;
    private IMapper _mapper;

    public OrderSuccessfulController(
        IOrderSuccessfulService ordersuccessfulService,
        IMapper mapper)
    {
        _ordersuccessful = ordersuccessfulService;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        return Ok(await _ordersuccessful.GetAll(param.Page, param.PageSize, param.SearchText));
    }


    [HttpGet("list")]
    public IActionResult GetSelectList()
    {
        var ordersucessful = _ordersuccessful.GetAll();

        return Ok(new BaseResponseModel
        {
            Data = ordersucessful,
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var model = _ordersuccessful.GetByID(id);
        return Ok(model);
    }

    [HttpPost]
    public IActionResult Create([FromBody] OrderSuccessful model)
    {
        // map model to entity and set id
        var ordersuccessful = _mapper.Map<OrderSuccessful>(model);
        try
        {
            _ordersuccessful.Create(ordersuccessful);

            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return Ok(new { code = 400, msg = ex.Message });
        }
    }
    [HttpPut("{id}")]
    public IActionResult Save(int id, [FromBody] OrderSuccessful model)
    {
        // map model to entity and set id
        var ordersuccessful = _mapper.Map<OrderSuccessful>(model);
        try
        {
            _ordersuccessful.Update(ordersuccessful);

            return Ok();
        }
        catch (ErrorException ex)
        {
            // return error message if there was an exception
            return Ok(new { code = 400, msg = ex.Message });
        }
    }
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _ordersuccessful.Delete(id);
        return Ok();
    }
}
