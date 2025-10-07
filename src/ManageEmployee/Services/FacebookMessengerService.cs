using ManageEmployee.DataTransferObject;
using ManageEmployee.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ManageEmployee.Services
{
    public class FacebookMessengerService : IFacebookMessengerService
    {
        private readonly string _pageAccessToken = "EAAb9cyvvhRoBOwiZAYTVngzInnhC0WBHJZBc3ZBElXkULq9pDOPkQFQ1dkkUTzBI7y8pz4JU13CidN5ZCTpXZCjuMO5crFrJkM0if4HmPGjXfp4dUtnFoOCTZAopZBpXGjYPMerTKz4YA8f98GyTZC20OguKDxFNBDlXZBZCAqsvb3fn60AgCUi4Q07edEP14VezTZCkrqZAg8rKs2qZB64fdHPPl";

        public async Task SendMessageAsync(string psid, string message)
        {
            var payload = new
            {
                recipient = new { id = psid },
                message = new { text = message },
                tag = "ACCOUNT_UPDATE",
                messaging_type = "MESSAGE_TAG"
            };

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsJsonAsync(
                $"https://graph.facebook.com/v18.0/me/messages?access_token={_pageAccessToken}",
                payload
            );

            response.EnsureSuccessStatusCode();
        }
        public async Task SendButtonTemplateAsync(string recipientId, string message, List<MessengerButton> buttons)
        {
            var buttonList = buttons.Select(b => new
            {
                type = "postback",
                title = b.Title,
                payload = b.Payload
            }).ToList();

            var payload = new
            {
                recipient = new { id = recipientId },
                message = new
                {
                    attachment = new
                    {
                        type = "template",
                        payload = new
                        {
                            template_type = "button",
                            text = message,
                            buttons = buttonList
                        }
                    }
                },
                tag = "ACCOUNT_UPDATE",
                messaging_type = "MESSAGE_TAG"
            };

            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(
                $"https://graph.facebook.com/v18.0/me/messages?access_token={_pageAccessToken}",
                content
            );

            response.EnsureSuccessStatusCode();
        }
    }
}
