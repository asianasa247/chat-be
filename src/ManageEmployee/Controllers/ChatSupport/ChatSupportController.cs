using ManageEmployee.DataTransferObject.ChatSupport;
using ManageEmployee.Services.Interfaces.ChatSupport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using ManageEmployee.Hubs;
using System.Security.Claims;

namespace ManageEmployee.Controllers.ChatSupport;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ChatSupportController : ControllerBase
{
    private readonly IChatSupportService _chat;
    private readonly IHubContext<ChatSupportHub> _hub;

    public ChatSupportController(IChatSupportService chat, IHubContext<ChatSupportHub> hub)
    {
        _chat = chat;
        _hub = hub;
    }

    private string CurrentUserId =>
        User?.Identity?.Name
        ?? User?.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? "unknown";

    private string? CurrentUserName =>
        User?.FindFirstValue("name")
        ?? User?.FindFirstValue(ClaimTypes.Name);

    [HttpPost("rooms")]
    public async Task<IActionResult> CreateRoom([FromBody] ChatSupportCreateRoomRequest req)
    {
        var room = await _chat.CreateRoomAsync(req.Name, req.Kind, req.ParticipantIds, CurrentUserId, CurrentUserName);
        return Ok(new { room.Id, room.Name, room.Kind });
    }

    [HttpGet("rooms/my")]
    public async Task<IActionResult> MyRooms()
    {
        var rooms = await _chat.GetMyRoomsAsync(CurrentUserId);
        return Ok(rooms.Select(r => new ChatSupportRoomModel
        {
            Id = r.Id,
            Name = r.Name,
            Kind = r.Kind,
            ParticipantIds = r.Participants.Select(p => p.UserId),
            CreateAt = r.CreatedAt
        }));
    }

    [HttpGet("rooms/{roomId:int}/messages")]
    public async Task<IActionResult> GetMessages([FromRoute] int roomId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
    {
        if (!await _chat.IsParticipantAsync(roomId, CurrentUserId))
            return Forbid();

        var items = await _chat.GetMessagesAsync(roomId, page, pageSize);
        return Ok(items.Select(m => new ChatSupportMessageModel
        {
            Id = m.Id,
            RoomId = m.RoomId,
            SenderId = m.SenderId,
            SenderName = m.SenderName,
            Content = m.Content,
            AttachmentUrl = m.AttachmentUrl,
            ContentType = m.ContentType,
            SentAt = m.SentAt,
            IsEdited = m.IsEdited,
            EditedAt = m.EditedAt
        }));
    }

    // ==== NEW: gửi tin nhắn qua REST để test trên Swagger ====
    [HttpPost("rooms/{roomId:int}/messages")]
    public async Task<IActionResult> SendMessage([FromRoute] int roomId, [FromBody] ChatSupportSendMessageRequest body)
    {
        // Bảo vệ: chỉ participant mới được gửi
        if (!await _chat.IsParticipantAsync(roomId, CurrentUserId))
            return Forbid();

        var saved = await _chat.AddMessageAsync(
            roomId,
            CurrentUserId,
            CurrentUserName,
            body.Content,
            body.AttachmentUrl,
            string.IsNullOrWhiteSpace(body.ContentType) ? "text" : body.ContentType
        );

        // Broadcast ra SignalR cho client đang join nhóm room
        await _hub.Clients.Group($"room:{roomId}").SendAsync("messageReceived", new
        {
            saved.Id,
            saved.RoomId,
            saved.SenderId,
            saved.SenderName,
            saved.Content,
            saved.AttachmentUrl,
            saved.ContentType,
            saved.SentAt
        });

        return Ok(new ChatSupportMessageModel
        {
            Id = saved.Id,
            RoomId = saved.RoomId,
            SenderId = saved.SenderId,
            SenderName = saved.SenderName,
            Content = saved.Content,
            AttachmentUrl = saved.AttachmentUrl,
            ContentType = saved.ContentType,
            SentAt = saved.SentAt,
            IsEdited = saved.IsEdited,
            EditedAt = saved.EditedAt
        });
    }
}
