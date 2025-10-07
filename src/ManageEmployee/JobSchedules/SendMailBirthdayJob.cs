using Common.Errors;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.Enumerations;
using ManageEmployee.Helpers;
using ManageEmployee.JobSchedules.Interface;
using ManageEmployee.Services.Interfaces.Mails;

namespace ManageEmployee.JobSchedules;

public class SendMailBirthdayJob : ISendMailBirthdayJob
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ISendMailService _sendMailService;

    public SendMailBirthdayJob(ApplicationDbContext dbContext, ISendMailService sendMailService)
    {
        _dbContext = dbContext;
        _sendMailService = sendMailService;
    }

    public void SendMail()
    {
        try
        {
            var customerEmails = _dbContext.Customers.Where(t => !string.IsNullOrEmpty(t.Email)
                                  && t.Birthday != null && t.Birthday.Value.Day == DateTime.Now.Day && t.Birthday.Value.Month == DateTime.Now.Month).Select(x => x.Email).ToList();
            if (customerEmails.Any())
            {
                var mailContent = _dbContext.SendMails.FirstOrDefault(x => x.Type == nameof(SendMailType.Birthday));
                if (mailContent is null)
                {
                    throw new ErrorException(ErrorMessage.DATA_IS_EMPTY);
                }
                foreach (var email in customerEmails)
                {
                    _sendMailService.SendEmail(mailContent, email);
                }
            }
        }
        catch (Exception ex)
        {
            throw new ErrorException(ex.Message);
        }
    }
}