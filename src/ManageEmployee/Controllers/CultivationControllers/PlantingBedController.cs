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
    public class PlantingBedController : ControllerBase
    {
        private readonly IPlantingBedService _service;
        private readonly IMapper _mapper;

        public PlantingBedController(IPlantingBedService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param, [FromQuery] int? regionId, [FromQuery] int? typeId)
            => Ok(await _service.GetAll(param.Page, param.PageSize, param.SearchText, regionId, typeId));

        [HttpGet("list")]
        public IActionResult List() => Ok(new BaseResponseModel { Data = _service.GetAll() });

        [HttpGet("{id}")]
        public IActionResult GetById(int id) => Ok(_service.GetById(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PlantingBed model)
        {
            try
            {
                var result = await _service.Create(model);
                if (string.IsNullOrEmpty(result)) return Ok();
                return BadRequest(new { msg = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.InnerException?.Message ?? ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PlantingBed model)
        {
            try
            {
                if (id <= 0) return BadRequest(new { msg = "Invalid id" });
                model.Id = id;

                var result = await _service.Update(model);
                if (string.IsNullOrEmpty(result)) return Ok();
                return BadRequest(new { msg = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.InnerException?.Message ?? ex.Message });
            }
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
