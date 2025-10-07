using AutoMapper;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.ChatboxAI;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.ChatboxAI;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.ChatboxAIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatboxAIScheduledMessageController : ControllerBase
    {
        private readonly IChatboxAIScheduledMessageService _service;
        private readonly IMapper _mapper;

        public ChatboxAIScheduledMessageController(IChatboxAIScheduledMessageService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param, [FromQuery] int? topicId)
        {
            return Ok(await _service.GetAll(param.Page, param.PageSize, param.SearchText, topicId));
        }

        [HttpGet("list")]
        public IActionResult List()
        {
            var results = _service.GetAll();
            return Ok(new BaseResponseModel { Data = results });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var model = _service.GetById(id);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ChatboxAIScheduledMessage model)
        {
            try
            {
                var result = await _service.Create(model);
                if (string.IsNullOrEmpty(result)) return Ok();
                return Ok(new { code = 400, msg = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ChatboxAIScheduledMessage model)
        {
            try
            {
                if (id <= 0) return BadRequest(new { msg = "Invalid id" });

                // Ép id từ route, không tin cậy id trong body
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
            try
            {
                var result = _service.Delete(id);
                if (string.IsNullOrEmpty(result)) return Ok();
                return BadRequest(new { msg = result });
            }
            catch (ErrorException ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }
    }
}
