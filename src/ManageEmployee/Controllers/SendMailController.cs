using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ManageEmployee.Services.Interfaces.Mails;
using ManageEmployee.DataTransferObject.PagingRequest;
using ManageEmployee.Entities;
using ManageEmployee.DataTransferObject.BaseResponseModels;
using ManageEmployee.JobSchedules.Interface;

namespace ManageEmployee.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SendMailController : ControllerBase
{
    private readonly ISendMailService _sendMailService;
    private readonly ISendMailBirthdayJob _sendMailBirthdayJob;
    public SendMailController(ISendMailService sendMailService, ISendMailBirthdayJob sendMailBirthdayJob)
    {
        _sendMailService = sendMailService;
        _sendMailBirthdayJob = sendMailBirthdayJob;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] PagingRequestModel param)
    {
        var data = await _sendMailService.GetAll(param);
        return Ok(new BaseResponseModel
        {
            TotalItems = data.Count(),
            Data = data.Skip(param.PageSize * (param.Page - 1))
             .Take(param.PageSize),
            PageSize = param.PageSize,
            CurrentPage = param.Page
        });
    }
    [HttpPost]
    public async Task<IActionResult> Create(SendMail request)
    {
        await _sendMailService.Create(request);
        return Ok();
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(SendMail request, int id)
    {
        request.Id = id;
        await _sendMailService.Update(request);
        return Ok();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _sendMailService.Delete(id);
        return Ok();
    }
    [HttpGet("job-send-mail-birthday")]
    public IActionResult JobSendMailBirthday()
    {
        RecurringJob.AddOrUpdate( () => _sendMailBirthdayJob.SendMail(), Cron.Daily(9, 0));
        return Ok();
    }
}
