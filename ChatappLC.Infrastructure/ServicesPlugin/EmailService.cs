using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Configuration;

namespace ChatappLC.Infrastructure.ServicesPlugin;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private bool IsValidEmail(string email)
    {
        var emailRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailRegex);
    }

    public async Task<bool> VerifyEmailWithMailboxValidator(string email)
    {
        if (string.IsNullOrEmpty(email) || !IsValidEmail(email))
        {
            return false;
        }

        var apiKey = _configuration["EmailSettings:MailboxValidatorApiKey"]; // moved key to config
        var url = $"https://api.mailboxvalidator.com/v1/validation/single?key={apiKey}&email={email}";

        using (var client = new HttpClient())
        {
            try
            {
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var rawContent = await response.Content.ReadAsStringAsync();

                    using (JsonDocument doc = JsonDocument.Parse(rawContent))
                    {
                        if (doc.RootElement.TryGetProperty("is_verified", out JsonElement isVerifiedElement))
                        {
                            string isVerified = isVerifiedElement.GetString();
                            return isVerified == "True";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email verification error: {ex.Message}");
            }
        }

        return false;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
    {
        var smtpHost = _configuration["EmailSettings:SmtpHost"];       // e.g. smtp.gmail.com
        var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]); // e.g. 587
        var fromEmail = _configuration["EmailSettings:FromEmail"];
        var password = _configuration["EmailSettings:Password"];

        try
        {
            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true
            };

            var message = new MailMessage(fromEmail, toEmail, subject, htmlContent)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(message);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email to {toEmail}: {ex.Message}");
            throw; // optional: throw again if you want controller to handle it
        }
    }
}
