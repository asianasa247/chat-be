namespace ChatappLC.Domain.Entities;

public class Message
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public required string SenderId { get; set; }
    public string? ReceiverId { get; set; } // for 1-1 chat
    public string? GroupId { get; set; }    // for group chat

    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;

    public bool IsEdited { get; set; } = false;

    public bool IsDeletedForEveryone { get; set; } = false;
    public List<string> DeletedForUserIds { get; set; } = new();
}
