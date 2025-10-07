using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.Email;
using ManageEmployee.Services.Interfaces.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.EmailControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailLogController : ControllerBase
    {
        IMailLogService _mailLogService;
        public MailLogController(IMailLogService mailLogService)
        {
            _mailLogService = mailLogService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mailLogService.GetAll();
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mailLogService.GetById(id);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MailLogModel model)
        {
            var result = await _mailLogService.Create(model);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] MailLogModel model)
        {
            var result = await _mailLogService.Update(model);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mailLogService.Delete(id);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
    }
}
