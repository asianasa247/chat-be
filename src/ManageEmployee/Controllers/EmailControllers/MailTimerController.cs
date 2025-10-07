using AutoMapper;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.DataTransferObject.Email;
using ManageEmployee.Services.Interfaces.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ManageEmployee.Controllers.EmailControllers
{
    [Route("api/[controller]")]
    [ApiController]
#if DEBUG
    [AllowAnonymous]
#endif
    public class MailTimerController : ControllerBase
    {
        IMailTimerService _mailTimerService;
        IMailTemplateService _mailTemplateService;
        IMapper _mapper;
        public MailTimerController(IMailTimerService mailTimerService, IMailTemplateService mailTemplateService, IMapper mapper)
        {
            _mailTimerService = mailTimerService;
            _mailTemplateService = mailTemplateService;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mailTimerService.GetAll();
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpGet("MailTimerTemplate")]
        public async Task<IActionResult> GetMailTimerTemplate()
        {
            var result = await _mailTimerService.GetAll();
            var templateIds = result.Select(x => x.MailTemplateId).ToList();
            var templates = _mapper.Map<List<MailTemplateModel>>(await _mailTemplateService.GetByListId(templateIds));
            var _res = result.Select(x =>
            {
                var t = templates.FirstOrDefault(z => z.Id == x.MailTemplateId);
                var model = _mapper.Map<MailTimerTemplate>(x);
                model.MailTemplate = t;
                return model;
            });
            return Ok(new BaseResponseModel
            {
                Data = _res
            });
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mailTimerService.GetById(id);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MailTimerModel model)
        {
            var result = await _mailTimerService.Create(model);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] MailTimerModel model)
        {
            var result = await _mailTimerService.Update(model);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mailTimerService.Delete(id);
            return Ok(new BaseResponseModel
            {
                Data = result
            });
        }
    }
}
