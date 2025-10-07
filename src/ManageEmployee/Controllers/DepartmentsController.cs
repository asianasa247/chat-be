using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Departments;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Entities.Constants;
using ManageEmployee.Entities.AddressEntities;
using ManageEmployee.DataTransferObject;
using ManageEmployee.DataTransferObject.BaseResponseModels;

namespace ManageEmployee.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(
        IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DepartmentRequest param)
    {
        return Ok(await _departmentService.GetAll(param));
    }

    [HttpGet("list")]
    public IActionResult GetSelectList()
    {
        var departments = _departmentService.GetAll();
        return Ok(new BaseResponseModel
        {
            Data = departments,
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var model = _departmentService.GetById(id);
        return Ok(model);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Department department)
    {
        try
        {
            bool check = _departmentService.checkMemberHaveWarehouseCode(null, department.Code);
            if (check)
            {
                return Ok(
                   new ObjectReturn
                   {
                       message = ResultErrorConstants.DEPARTMENT_CODE_IS_EXIST,
                       status = Convert.ToInt32(ErrorEnum.DEPARTMENT_CODE_IS_EXIST)
                   });
            }
            _departmentService.Create(department);
            return Ok(new ObjectReturn
            {
                status = 200,
                data = department
            });
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Department department)
    {
        try
        {
            bool check = _departmentService.checkMemberHaveWarehouseCode(id, department.Code);
            if (check)
            {
                return Ok(
                   new ObjectReturn
                   {
                       message = ResultErrorConstants.DEPARTMENT_CODE_IS_EXIST,
                       status = Convert.ToInt32(ErrorEnum.DEPARTMENT_CODE_IS_EXIST)
                   });
            }
            Department result = _departmentService.Update(id, department);
            return Ok(new ObjectReturn
            {
                status = 200,
                data = result
            });
        }
        catch (ErrorException ex)
        {
            return BadRequest(new { msg = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        bool check = _departmentService.checkMemberHaveWarehouse(id);
        if (check)
        {
            return Ok(
               new ObjectReturn
               {
                   message = ResultErrorConstants.DEPARTMENT_IS_USE,
                   status = Convert.ToInt32(ErrorEnum.DEPARTMENT_IS_USE)
               });
        }
        _departmentService.Delete(id);
        return Ok();
    }

    [HttpGet("get-list-department-for-task")]
    public async Task<IActionResult> GetListDepartmentForTask()
    {
        if (HttpContext.User.Identity is ClaimsIdentity identity)
        {
            int userId = int.Parse(identity.Claims.First(x => x.Type == "UserId").Value);

            var result = await _departmentService.GetListDepartmentForTask(userId);
            if (result != null)
                return Ok(new BaseResponseModel
                {
                    Data = result,
                });
            else
                return NotFound(result);
        }
        else
        {
            return BadRequest("Bạn không có quyền truy cập.");
        }
    }
}