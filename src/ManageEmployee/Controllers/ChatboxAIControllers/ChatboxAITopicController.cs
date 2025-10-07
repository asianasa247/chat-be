using AutoMapper;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.ChatbotAI;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities.ChatboxAI;
using ManageEmployee.Entities.MenuEntities;
using ManageEmployee.Helpers;
using ManageEmployee.Services.Interfaces.Assets;
using ManageEmployee.Services.Interfaces.ChatboxAI;
using ManageEmployee.Services.Interfaces.Menus;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.ChatboxAIControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatboxAITopicController : ControllerBase
    {
        private IChatboxAITopicService _ChatboxAITopicService;
        private IMapper _mapper;

        public ChatboxAITopicController(
            IChatboxAITopicService ChatboxAITopicService,
            IMapper mapper, IFileService fileService)
        {
            _ChatboxAITopicService = ChatboxAITopicService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
        {

            return Ok(await _ChatboxAITopicService.GetAll(param.Page, param.PageSize, param.SearchText));
        }

        [HttpGet("list")]
        public IActionResult GetChatboxAITopic()
        {
            var results = _ChatboxAITopicService.GetAll();

            return Ok(new BaseResponseModel
            {
                Data = results,
            });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var model = _ChatboxAITopicService.GetById(id);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ChatboxAITopic model)
        {
            try
            {
                var result = await _ChatboxAITopicService.Create(model);
                if (string.IsNullOrEmpty(result))
                    return Ok();
                return Ok(new { code = 400, msg = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] ChatboxAITopic model)
        {
            try
            {

                var result = await _ChatboxAITopicService.Update(model);
                if (string.IsNullOrEmpty(result))
                    return Ok();
                return Ok(new { code = 400, msg = result });
            }
            catch (ErrorException ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var result = _ChatboxAITopicService.Delete(id);
                if (string.IsNullOrEmpty(result))
                    return Ok();
                return BadRequest(new { msg = result });
            }
            catch (ErrorException ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }
    }
}
