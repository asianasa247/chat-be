namespace ChatappLC.Infrastructure.ServicesPlugin;

public interface IEmailService
{
    Task<bool> VerifyEmailWithMailboxValidator(string email);
}
