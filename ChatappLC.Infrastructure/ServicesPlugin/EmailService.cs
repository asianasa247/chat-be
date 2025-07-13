using System.Text.Json;
using System.Text.RegularExpressions;

namespace ChatappLC.Infrastructure.ServicesPlugin;

internal class EmailService : IEmailService
{
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

        var apiKey = "4PNRFN4UXN8TU129E55A";
        var url = $"https://api.mailboxvalidator.com/v1/validation/single?key={apiKey}&email={email}";

        Console.WriteLine($"Starting email validation for {email}");

        using (var client = new HttpClient())
        {
            var response = await client.GetAsync(url);

            Console.WriteLine($"Response: {response}");

            if (response.IsSuccessStatusCode)
            {
                var rawContent = await response.Content.ReadAsStringAsync();

                // Sử dụng JsonDocument để phân tích JSON và lấy giá trị của IsVerified
                using (JsonDocument doc = JsonDocument.Parse(rawContent))
                {
                    if (doc.RootElement.TryGetProperty("is_verified", out JsonElement isVerifiedElement))
                    {
                        string isVerified = isVerifiedElement.GetString();
                        Console.WriteLine($"Parsed IsVerified value: {isVerified}");
                        return isVerified == "True";
                    }
                    else
                    {
                        Console.WriteLine("Error: 'is_verified' property not found in JSON response.");
                    }
                }
            }
            else
            {
                Console.WriteLine($"Failed to validate email {email}. StatusCode: {response.StatusCode}");
            }
        }
        return false;
    }
}
