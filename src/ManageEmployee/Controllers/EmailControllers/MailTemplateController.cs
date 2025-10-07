using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.Email;
using ManageEmployee.Services.Interfaces.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.EmailControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailTemplateController : ControllerBase
    {
        IMailTemplateService _mailTemplateService;
        public MailTemplateController(IMailTemplateService mailTemplateService)
        {
            _mailTemplateService = mailTemplateService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mailTemplateService.GetAll();
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mailTemplateService.GetById(id);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MailTemplateModel model)
        {
            var result = await _mailTemplateService.Create(model);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] MailTemplateModel model)
        {
            var result = await _mailTemplateService.Update(model);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mailTemplateService.Delete(id);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
    }
}
