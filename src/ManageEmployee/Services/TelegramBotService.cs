using ManageEmployee.Services.Interfaces;
using System.Text;
using System.Text.Json;

namespace ManageEmployee.Services
{
    public class TelegramBotService : ITelegramBotService
    {
        private readonly string _botToken = "8098779620:AAHZ3Z7O36MvssEQDIfd_1Mz2awBXmt2v6c"; // Replace with your bot token
        private static readonly HttpClient _httpClient = new HttpClient();

        public async Task SendMessageAsync(string chatId, string message)
        {
            var url = $"https://api.telegram.org/bot{_botToken}/sendMessage";
            var payload = new
            {
                chat_id = chatId,
                text = message
            };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();
        }
    }
}
