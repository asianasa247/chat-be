using ManageEmployee.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.ChatSupport;

public class ChatSupportParticipant : BaseEntity
{
    public int Id { get; set; }

    public int RoomId { get; set; }
    public ChatSupportRoom Room { get; set; } = default!;

    [MaxLength(64)]
    public string UserId { get; set; } = default!;

    [MaxLength(128)]
    public string? DisplayName { get; set; }

    public bool IsOwner { get; set; }
}
