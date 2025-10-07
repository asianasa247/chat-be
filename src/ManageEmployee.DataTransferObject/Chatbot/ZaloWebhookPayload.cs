using System.Text.Json.Serialization;

namespace ManageEmployee.DataTransferObject.Chatbot
{
    // Payload Webhook Zalo OA (tối thiểu các trường cần dùng)
    public class ZaloWebhookPayload
    {
        [JsonPropertyName("event_name")]
        public string? EventName { get; set; }  // "user_send_text" ...

        [JsonPropertyName("sender")]
        public SenderInfo? Sender { get; set; }

        [JsonPropertyName("message")]
        public MessageInfo? Message { get; set; }

        public class SenderInfo
        {
            [JsonPropertyName("id")]
            public string? Id { get; set; }
        }

        public class MessageInfo
        {
            [JsonPropertyName("text")]
            public string? Text { get; set; }
        }
    }
}
