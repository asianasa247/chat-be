namespace ManageEmployee.DataTransferObject.ChatSupport;

public class ChatSupportCreateRoomRequest
{
    public string Name { get; set; } = "General";
    public List<string> ParticipantIds { get; set; } = new();
    public string Kind { get; set; } = "group"; // group | direct
}

public class ChatSupportSendMessageRequest
{
    public int RoomId { get; set; }
    public string Content { get; set; } = default!;
    public string? AttachmentUrl { get; set; }
    public string? ContentType { get; set; }
}

public class ChatSupportMessageModel
{
    public long Id { get; set; }
    public int RoomId { get; set; }
    public string SenderId { get; set; } = default!;
    public string? SenderName { get; set; }
    public string Content { get; set; } = default!;
    public string? AttachmentUrl { get; set; }
    public string? ContentType { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
}

public class ChatSupportRoomModel
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Kind { get; set; } = default!;
    public IEnumerable<string> ParticipantIds { get; set; } = Enumerable.Empty<string>();
    public DateTime? CreateAt { get; set; }
}
