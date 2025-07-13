namespace ChatappLC.Application.DTOs.Chat;

public class ChatRoomResponse
{
    public string Id { get; set; } = string.Empty;
    public string RoomName { get; set; } = string.Empty;
    public List<UserInfoDTO> Participants { get; set; } = new List<UserInfoDTO>();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string LastMessage { get; set; } = string.Empty;
    public DateTime? LastMessageTime { get; set; }
    public string CreatorId { get; set; } = string.Empty;
    public string CustomRoomId { get; set; } = string.Empty;
    public bool IsGroup { get; set; } = false;
    public string ImageGroup { get; set; } = string.Empty;
}
