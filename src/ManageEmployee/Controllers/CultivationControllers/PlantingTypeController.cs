using AutoMapper;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.Cultivation;
using ManageEmployee.Services.Interfaces.Cultivation;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.CultivationControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlantingTypeController : ControllerBase
    {
        private readonly IPlantingTypeService _service;
        private readonly IMapper _mapper;

        public PlantingTypeController(IPlantingTypeService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param, [FromQuery] int? category)
            => Ok(await _service.GetAll(param.Page, param.PageSize, param.SearchText, category));

        [HttpGet("list")]
        public IActionResult List() => Ok(new BaseResponseModel { Data = _service.GetAll() });

        [HttpGet("{id}")]
        public IActionResult GetById(int id) => Ok(_service.GetById(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PlantingType model)
        {
            var result = await _service.Create(model);
            if (string.IsNullOrEmpty(result)) return Ok();
            return BadRequest(new { msg = result });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PlantingType model)
        {
            if (id <= 0) return BadRequest(new { msg = "Invalid id" });
            model.Id = id;

            var result = await _service.Update(model);
            if (string.IsNullOrEmpty(result)) return Ok();
            return BadRequest(new { msg = result });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _service.Delete(id);
            if (string.IsNullOrEmpty(result)) return Ok();
            return BadRequest(new { msg = result });
        }
    }
}
