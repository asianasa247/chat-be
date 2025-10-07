using ManageEmployee.Services.Interfaces;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace ManageEmployee.Services
{
    public class MessengerServices : IMessengerServices
    {

        public async Task SendMessageAsync(string messageId, string messageText)
        {
            if (string.IsNullOrEmpty(messageId) || string.IsNullOrEmpty(messageText))
            {
                throw new ArgumentException("Message ID and message text cannot be null or empty.");
            }
            string pageAccessToken = "EAAb9cyvvhRoBO4eszej9dlh2tubjHH4DZCcLywolgb4ZAQDj1n0Ei7J7I9UkuSZB8iHmUs1PFn7hSjpsox7Rls68FSe47ufWmPwOwl61GXK2UZBZCQUa8GoLzDLa4Ro0cdtf4n2MODoTJ8uVqhNSpIT1QLkp0crPqdvjtZBm0k3Q3NkzshyMm571UcLvh3buBiiypsYHage62lu70qz9GQ3guy2wZDZD";

            var url = $"https://graph.facebook.com/v17.0/me/messages?access_token={pageAccessToken}";

            var payload = new
            {
                recipient = new { id = messageId },
                message = new { text = messageText },
                messaging_type = "RESPONSE"  // hoặc "MESSAGE_TAG" nếu ngoài 24h
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);

            using (var client = new HttpClient())
            using (var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json"))
            {
                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error sending message: {result}");
                }
                Console.WriteLine("Message sent successfully!");
            }
        }
        public async Task SendButtonTemplateAsync(string recipientId, string text, List<(string title, string payload)> buttons)
        {
            string pageAccessToken = "EAAb9cyvvhRoBO4eszej9dlh2tubjHH4DZCcLywolgb4ZAQDj1n0Ei7J7I9UkuSZB8iHmUs1PFn7hSjpsox7Rls68FSe47ufWmPwOwl61GXK2UZBZCQUa8GoLzDLa4Ro0cdtf4n2MODoTJ8uVqhNSpIT1QLkp0crPqdvjtZBm0k3Q3NkzshyMm571UcLvh3buBiiypsYHage62lu70qz9GQ3guy2wZDZD";
            var url = $"https://graph.facebook.com/v18.0/me/messages?access_token={pageAccessToken}";

            var buttonList = buttons.Select(b => new
            {
                type = "postback",
                title = b.title,
                payload = b.payload
            });

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
                            text = text,
                            buttons = buttonList
                        }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(payload);

            using (var client = new HttpClient())
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                var response = await client.PostAsync(url, content);
                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Error sending button template: {result}");
                }

                Console.WriteLine("Button template sent successfully!");
            }
        }
    }
}
