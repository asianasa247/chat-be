using ManageEmployee.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.ChatSupport;

public class ChatSupportMessage : BaseEntity
{
    public long Id { get; set; }

    public int RoomId { get; set; }
    public ChatSupportRoom Room { get; set; } = default!;

    [MaxLength(64)]
    public string SenderId { get; set; } = default!;

    [MaxLength(128)]
    public string? SenderName { get; set; }

    [MaxLength(4096)]
    public string Content { get; set; } = default!;

    public string? AttachmentUrl { get; set; }

    [MaxLength(64)]
    public string? ContentType { get; set; }

    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
}
