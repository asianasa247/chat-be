using ManageEmployee.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class WebhookController : ControllerBase
{
    private const string VERIFY_TOKEN = "02062025TOKEN_ASIANASA";
    private readonly IMessengerServices _messengerServices;

    public WebhookController(IMessengerServices messengerServices)
    {
        _messengerServices = messengerServices;
    }

    [HttpGet]
    public IActionResult VerifyWebhook(
    [FromQuery(Name = "hub.mode")] string mode,
    [FromQuery(Name = "hub.verify_token")] string token,
    [FromQuery(Name = "hub.challenge")] string challenge)
    {
        if (mode == "subscribe" && token == VERIFY_TOKEN)
        {
            Console.WriteLine("✅ Webhook verified!");
            return Ok(challenge); // Trả về hub.challenge cho Facebook
        }

        return Forbid(); // Facebook sẽ nhận 403 nếu token không đúng
    }


    [HttpPost]
    public async Task<IActionResult> ReceiveWebhook([FromBody] JObject body)
    {
        if (body["object"]?.ToString() != "page")
            return NotFound();

        foreach (var entry in body["entry"])
        {
            foreach (var messageEvent in entry["messaging"])
            {
                var senderId = messageEvent["sender"]?["id"]?.ToString();
                var messageText = messageEvent["message"]?["text"]?.ToString();
                var postbackPayload = messageEvent["postback"]?["payload"]?.ToString();

                // 👉 Xử lý khi người dùng bấm nút
                if (!string.IsNullOrEmpty(postbackPayload))
                {
                    if (postbackPayload == "ACTION_GET_ID")
                    {
                        await _messengerServices.SendMessageAsync(senderId, $"Bot nhận được rồi! ID của bạn là: {senderId}");
                    }
                    else if (postbackPayload == "ACTION_GET_TASK")
                    {
                        await _messengerServices.SendMessageAsync(senderId, "Cảm ơn bạn, nhiệm vụ sẽ được thông báo tới bạn nếu có.");
                    }
                    else if (postbackPayload.StartsWith("REMINDER_READ_"))
                    {
                        await _messengerServices.SendMessageAsync(senderId,
                            "✅ Cảm ơn bạn đã xem nhiệm vụ. Những nhiệm vụ tiếp theo sẽ được gửi đến bạn.");
                    }

                    continue; // Không cần xử lý tiếp messageText nếu đã xử lý postback
                }

                // 👉 Nếu là tin nhắn văn bản bình thường
                if (!string.IsNullOrEmpty(senderId) && !string.IsNullOrEmpty(messageText))
                {
                    await _messengerServices.SendButtonTemplateAsync(
                        senderId,
                        "Bạn muốn thực hiện hành động gì?",
                        new List<(string title, string payload)>
                        {
                            ("Nhận ID", "ACTION_GET_ID"),
                            ("Nhận Nhiệm Vụ", "ACTION_GET_TASK")
                        }
                    );
                }
            }
        }

        return Ok("EVENT_RECEIVED");
    }
}
