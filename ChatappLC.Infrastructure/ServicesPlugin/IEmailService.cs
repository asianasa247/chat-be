namespace ChatappLC.Infrastructure.ServicesPlugin;

public interface IEmailService
{
    Task<bool> VerifyEmailWithMailboxValidator(string email);
    Task SendEmailAsync(string toEmail, string subject, string htmlContent);
}
