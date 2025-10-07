using ManageEmployee.Services.Interfaces.ChatSupport;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ManageEmployee.Hubs;

[Authorize]
public class ChatSupportHub : Hub
{
    private readonly IChatSupportService _chatService;

    public ChatSupportHub(IChatSupportService chatService)
    {
        _chatService = chatService;
    }

    private string CurrentUserId =>
        Context.User?.Identity?.Name
        ?? Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? Context.UserIdentifier
        ?? "unknown";

    private string? CurrentUserName =>
        Context.User?.FindFirst("name")?.Value
        ?? Context.User?.FindFirst(ClaimTypes.Name)?.Value;

    public async Task JoinRoom(int roomId)
    {
        if (!await _chatService.IsParticipantAsync(roomId, CurrentUserId))
            throw new HubException("Not a participant.");
        await Groups.AddToGroupAsync(Context.ConnectionId, $"room:{roomId}");
    }

    public Task LeaveRoom(int roomId) =>
        Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room:{roomId}");

    public async Task SendMessage(int roomId, string content, string? attachmentUrl = null, string? contentType = null)
    {
        if (!await _chatService.IsParticipantAsync(roomId, CurrentUserId))
            throw new HubException("Not a participant.");

        var saved = await _chatService.AddMessageAsync(roomId, CurrentUserId, CurrentUserName, content, attachmentUrl, contentType);

        await Clients.Group($"room:{roomId}").SendAsync("messageReceived", new
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
    }
}
