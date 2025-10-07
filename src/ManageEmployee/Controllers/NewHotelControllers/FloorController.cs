using Emgu.CV.Ocl;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.AreaModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.AreaEntities;
using ManageEmployee.Services.Interfaces.NewHotels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace ManageEmployee.Controllers.NewHotelControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FloorController : ControllerBase
    {
        private readonly IFloorService _floorService;

        public FloorController(IFloorService floorService)
        {
            _floorService = floorService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] FloorDTO model)
        {
            var result = await _floorService.Add(model);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] FloorDTO model)
        {
            var result = await _floorService.Update(id, model);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var success = await _floorService.Delete(id);
            if (!success) return NotFound();
            return Ok();
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var result = await _floorService.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _floorService.GetById(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("GetPaged")]
        public async Task<IActionResult> GetPaged([FromQuery] PagingRequestModel pagingRequest)
        {
            try
            {
                var result = await _floorService.GetPaged(pagingRequest);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

}
