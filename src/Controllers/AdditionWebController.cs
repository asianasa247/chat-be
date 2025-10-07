using ManageEmployee.Entities;
using ManageEmployee.DataTransferObject;
using ManageEmployee.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ManageEmployee.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdditionWebController : ControllerBase
    {
        private readonly IAdditrionWebService _service;

        public AdditionWebController(IAdditrionWebService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdditionWeb>>> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AdditionWeb>> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<AdditionWeb>> AddOrUpdate([FromBody] AdditionWebModel model)
        {
            var result = await _service.AddOrUpdateAsync(model);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AdditionWeb>> Update(int id, [FromBody] AdditionWebModel model)
        {
            var result = await _service.UpdateAsync(id, model);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}