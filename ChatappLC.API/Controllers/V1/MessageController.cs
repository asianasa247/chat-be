using ChatappLC.Application.DTOs.Messages;
using ChatappLC.Application.Interfaces.Messages;
using ChatappLC.Domain.Entities;
using ChatappLC.Infrastructure.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatappLC.API.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageController(IMessageService messageService, IHubContext<ChatHub> hubContext)
        {
            _messageService = messageService;
            _hubContext = hubContext;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            var message = new Message
            {
                SenderId = request.SenderId,
                ReceiverId = request.ReceiverId,
                GroupId = request.GroupId,
                Content = request.Content,
                SentAt = DateTime.UtcNow
            };

            var result = await _messageService.SendMessageAsync(message);

            await _hubContext.Clients.User(message.ReceiverId!).SendAsync("MessageSent", result);
            await _hubContext.Clients.User(message.SenderId).SendAsync("MessageSent", result);

            return Ok(result);
        }


        [HttpPut("edit/{id}")]
        public async Task<IActionResult> EditMessage(string id, [FromQuery] string userId, [FromBody] string newContent)
        {
            var result = await _messageService.EditMessageAsync(id, userId, newContent);

            // 🔴 Gửi realtime đến người gửi và người nhận
            if (result != null)
            {
                await _hubContext.Clients.User(result.SenderId).SendAsync("MessageEdited", result);
                await _hubContext.Clients.User(result.ReceiverId!).SendAsync("MessageEdited", result);
            }

            return Ok(result);
        }

        [HttpDelete("delete-for-everyone/{id}")]
        public async Task<IActionResult> DeleteForEveryone(string id, [FromQuery] string userId)
        {
            var result = await _messageService.DeleteMessageForEveryoneAsync(id, userId);

            if (result != null)
            {
                await _hubContext.Clients.User(result.SenderId).SendAsync("MessageDeleted", id);
                await _hubContext.Clients.User(result.ReceiverId!).SendAsync("MessageDeleted", id);
            }

            return Ok(result != null);
        }

        [HttpDelete("delete-for-me/{id}")]
        public async Task<IActionResult> DeleteForMe(string id, [FromQuery] string userId)
        {
            var result = await _messageService.DeleteMessageForUserAsync(id, userId);

            if (result != null)
            {
                await _hubContext.Clients.User(userId).SendAsync("MessageDeletedForUser", id);
            }

            return Ok(result != null);
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetMessages([FromQuery] string userId, [FromQuery] string? receiverId, [FromQuery] string? groupId)
        {
            var result = await _messageService.GetMessagesAsync(userId, receiverId, groupId);
            return Ok(result);
        }
        [HttpGet("history/{userId}")]
        public async Task<IActionResult> GetMessageHistory(string userId)
        {
            var result = await _messageService.GetMessageHistoryForUserAsync(userId);
            return Ok(result);
        }

    }
}
