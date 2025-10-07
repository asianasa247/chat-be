using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.ChatSupport;
using ManageEmployee.Services.Interfaces.ChatSupport;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ManageEmployee.Services.ChatSupport;

public class ChatSupportEfService : IChatSupportService
{
    private readonly ApplicationDbContext _db;

    public ChatSupportEfService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ChatSupportRoom> CreateRoomAsync(string name, string kind, IEnumerable<string> participantIds, string ownerId, string? ownerName)
    {
        var room = new ChatSupportRoom
        {
            Name = string.IsNullOrWhiteSpace(name) ? "General" : name,
            Kind = string.IsNullOrWhiteSpace(kind) ? "group" : kind,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.ChatSupportRooms.Add(room);
        await _db.SaveChangesAsync();

        var uniqueIds = participantIds.Concat(new[] { ownerId })
                                      .Where(x => !string.IsNullOrWhiteSpace(x))
                                      .Distinct();

        foreach (var uid in uniqueIds)
        {
            _db.ChatSupportParticipants.Add(new ChatSupportParticipant
            {
                RoomId = room.Id,
                UserId = uid,
                DisplayName = uid == ownerId ? ownerName : null,
                IsOwner = uid == ownerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }
        await _db.SaveChangesAsync();
        return room;
    }

    public async Task<IEnumerable<ChatSupportRoom>> GetMyRoomsAsync(string userId)
    {
        // FIX: Include phải đứng TRƯỚC Select; query từ Rooms và lọc bằng Any()
        return await _db.ChatSupportRooms
            .Where(r => r.Participants.Any(p => p.UserId == userId))
            .Include(r => r.Participants)
            .OrderByDescending(r => r.UpdatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ChatSupportMessage>> GetMessagesAsync(int roomId, int page = 1, int pageSize = 50)
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 50;

        return await _db.ChatSupportMessages
            .Where(m => m.RoomId == roomId)
            .OrderByDescending(m => m.SentAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .OrderBy(m => m.SentAt)
            .ToListAsync();
    }

    public async Task<ChatSupportMessage> AddMessageAsync(int roomId, string senderId, string? senderName, string content, string? attachmentUrl, string? contentType)
    {
        var isIn = await _db.ChatSupportParticipants.AnyAsync(p => p.RoomId == roomId && p.UserId == senderId);
        if (!isIn) throw new InvalidOperationException("Not a participant.");

        var msg = new ChatSupportMessage
        {
            RoomId = roomId,
            SenderId = senderId,
            SenderName = senderName,
            Content = content,
            AttachmentUrl = attachmentUrl,
            ContentType = contentType,
            SentAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _db.ChatSupportMessages.Add(msg);

        var room = await _db.ChatSupportRooms.FirstAsync(r => r.Id == roomId);
        room.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();
        return msg;
    }

    public Task<bool> IsParticipantAsync(int roomId, string userId)
        => _db.ChatSupportParticipants.AnyAsync(p => p.RoomId == roomId && p.UserId == userId);
}
