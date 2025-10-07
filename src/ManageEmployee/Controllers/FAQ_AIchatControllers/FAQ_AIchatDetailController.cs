using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Services.Interfaces.FAQ_AIchat;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.FAQ_AIchatControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FAQ_AIchatDetailController : ControllerBase
    {
        private readonly IFAQ_AIchatDetailService _faqAIchatDetailService;
        public FAQ_AIchatDetailController(IFAQ_AIchatDetailService faqAIchatDetailService)
        {
            _faqAIchatDetailService = faqAIchatDetailService;
        }

        [HttpGet("index")]
        public async Task<IActionResult> GetChatDetails([FromQuery] PagingRequestForChatModel model)
        {
            var resultLogs = await _faqAIchatDetailService.GetAllByChatID(model.Page, model.PageSize, model.FAQ_AIchatId);
            return Ok(new BaseResponseModel
            {
                Data = resultLogs,
                CurrentPage = model.Page,
                PageSize = model.PageSize,
            });
        }
    }
}
