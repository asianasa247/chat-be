using ManageEmployee.Entities.BaseEntities;
using System.ComponentModel.DataAnnotations;

namespace ManageEmployee.Entities.ChatSupport;

public class ChatSupportRoom : BaseEntity
{
    public int Id { get; set; }

    [MaxLength(128)]
    public string Name { get; set; } = default!;     // Tên phòng

    [MaxLength(16)]
    public string Kind { get; set; } = "group";      // "group" | "direct"

    public bool IsArchived { get; set; }

    public ICollection<ChatSupportParticipant> Participants { get; set; } = new List<ChatSupportParticipant>();
    public ICollection<ChatSupportMessage> Messages { get; set; } = new List<ChatSupportMessage>();
}
