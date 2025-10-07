using ManageEmployee.DataTransferObject.ConvertProductModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.ConvertToProduct;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.ConvertProductControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConvertProductController : ControllerBase
    {
        private readonly IConvertProductService _convertProductService;

        public ConvertProductController(IConvertProductService convertProductService)
        {
            _convertProductService = convertProductService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] DTOConvert model)
        {
            var item = await _convertProductService.Add(model);
            return Ok(item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] DTOConvert model)
        {
            var item = await _convertProductService.Update(id, model);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _convertProductService.Delete(id);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _convertProductService.GetById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel request)
        {
            var result = await _convertProductService.GetAll(request);
            if (result == null) return BadRequest();
            return Ok(result);
        }
    }

}
