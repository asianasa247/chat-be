using ManageEmployee.DataTransferObject;
using ManageEmployee.Entities;
using ManageEmployee.Services.Interfaces.EmployessByOrder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeByOrderController : ControllerBase
    {
        private readonly IEmployeeByOrderService _employeeByOrderService;

        public EmployeeByOrderController(IEmployeeByOrderService employeeByOrderService)
        {
            _employeeByOrderService = employeeByOrderService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] EmployeeByOrderModels model)
        {
            var result = await _employeeByOrderService.Add(model);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeByOrderModels model)
        {
            var result = await _employeeByOrderService.Update(id, model);
            if (result == null) return NotFound("Employee not found.");
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _employeeByOrderService.Delete(id);
            if (!isDeleted) return NotFound("Employee not found.");
            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _employeeByOrderService.GetById(id);
            if (result == null) return NotFound("Employee not found.");
            return Ok(result);
        }
        [HttpGet("by-employee-id/{employeeId}")]
        public async Task<IActionResult> GetByEmployeeId(int employeeId)
        {
            var result = await _employeeByOrderService.GetByEmployeeId(employeeId);
            if (result == null) return null;
            return Ok(result);
        }
        [HttpGet("by-order-id/{orderId}")]
        public async Task<IActionResult> GetByOrderId(int orderId)
        {
            var result = await _employeeByOrderService.GetByOrderId(orderId);
            return Ok(result);
        }
    }
}
