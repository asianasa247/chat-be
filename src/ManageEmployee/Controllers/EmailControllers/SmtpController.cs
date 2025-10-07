using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.Smtp;
using ManageEmployee.Services.Interfaces.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.EmailController
{
    [Route("api/[controller]")]
    [ApiController]
#if DEBUG
    [AllowAnonymous]
#endif
    public class SmtpController : ControllerBase
    {
        private readonly ISmtpService _smtpService;
        public SmtpController(ISmtpService smtpService)
        {
            _smtpService = smtpService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _smtpService.GetAll();
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _smtpService.GetById(id);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SmtpModel model)
        {
            var result = await _smtpService.Create(model);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] SmtpModel model)
        {
            var result = await _smtpService.Update(model);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _smtpService.Delete(id);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
    }
}
