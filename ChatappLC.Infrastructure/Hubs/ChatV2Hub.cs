namespace ChatappLC.Infrastructure.Hubs;
//[Authorize]
public class ChatV2Hub : Hub
{
    private readonly IChatService _chatService;
    private readonly ILogger<ChatHub> _logger;
    public ChatV2Hub(IChatService chatService, ILogger<ChatHub> logger)
    {
        _chatService = chatService;
        _logger = logger;
    }

    // Phương thức để client gửi tin nhắn realtime
    public async Task SendMessage(ChatMessageRequest dto)
    {
        // Gọi service để xử lý tin nhắn (với logic tự kiểm tra/tạo ChatRoom nếu cần)
        var response = await _chatService.CreateChatMessageAsync(dto);

        if (response == null)
        {
            throw new Exception("Response from SendMessageAsync is null.");
        }

        if (response.Data == null)
        {
            throw new Exception("Response.Data is null.");
        }

        // Since response.Data is not a collection, handle it as a single instance
        var messageResponse = response.Data;

        if (messageResponse.ChatRoomId == null || string.IsNullOrEmpty(messageResponse.ChatRoomId))
        {
            // Có thể log hoặc ném exception nếu cần
            Console.WriteLine("ChatRoom is null or its Id is empty.");
            return;
        }

        // Broadcast tin nhắn đến tất cả client đang trong nhóm chat của phòng đó
        await Clients.Group(messageResponse.ChatRoomId).SendAsync("ReceiveMessage", messageResponse);
        // Sau khi cập nhật ChatRoom (trong SendMessage)
        var lastMessages = dto.MessageType == "text" ? dto.Content : dto.MessageType;
        await Clients.Group(messageResponse.ChatRoomId)
        .SendAsync("ReceiveRoomUpdate", new
        {
            chatRoomId = messageResponse.ChatRoomId,
            lastMessage = lastMessages,
            lastMessageTime = TimeZoneHelper.GetVietNamTimeNow()
        });

    }

    // Phương thức để client thu hồi tin nhắn
    public async Task RecallMessage(string messageId)
    {
        var response = await _chatService.RecallMessageAsync(messageId);

        if (response == null)
        {
            throw new Exception("Failed to recall message: response is null.");
        }

        if (!response.Flag)
        {
            throw new Exception("Failed to recall message: operation was not successful.");
        }
        // Broadcast thông báo thu hồi tin nhắn đến tất cả client trong nhóm chat của phòng đó
        await Clients.Group(response.Data.ChatRoomId).SendAsync("MessageRecalled", messageId);
    }


    // Phương thức để client xóa tin nhắn
    public async Task DeleteMessage(string messageId)
    {
        var response = await _chatService.DeleteMessageAsync(messageId);

        if (response == null)
        {
            throw new Exception("Failed to delete message. Response is null.");
        }

        if (!response.Flag)
        {
            throw new Exception("Failed to delete message. Operation was not successful.");
        }
        // Broadcast thông báo xóa tin nhắn đến tất cả client trong nhóm chat của phòng đó
        await Clients.Group(response.Data.ChatRoomId).SendAsync("MessageDeleted", messageId);
    }

    // Cho phép client tham gia nhóm chat dựa trên ChatRoomId
    public async Task JoinRoom(string chatRoomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomId);
    }

    // Cho phép client rời nhóm chat
    public async Task LeaveRoom(string chatRoomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomId);
    }
}
