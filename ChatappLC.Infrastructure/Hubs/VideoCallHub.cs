namespace ChatappLC.Infrastructure.Hubs;
[Authorize]
public class VideoCallHub : Hub
{
    private string? GetCurrentUserId()
    {
        return Context.User?.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;
    }

    public override async Task OnConnectedAsync()
    {
        var userId = GetCurrentUserId();
        if (!string.IsNullOrEmpty(userId))
        {
            Context.Items["userId"] = userId;
            Console.WriteLine($"[SignalR] User connected: {userId} with ConnectionId: {Context.ConnectionId}");
        }
        else
        {
            Console.WriteLine($"[SignalR] User connected: UNKNOWN with ConnectionId: {Context.ConnectionId}");
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        if (Context.Items.TryGetValue("userId", out var userId))
        {
            Console.WriteLine($"[SignalR] User disconnected: {userId} with ConnectionId: {Context.ConnectionId}");
        }
        else
        {
            Console.WriteLine($"[SignalR] User disconnected: UNKNOWN with ConnectionId: {Context.ConnectionId}");
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendOffer(string targetUserId, string offer)
    {
        var senderId = Context.Items["userId"];
        Console.WriteLine($"[SignalR] Sending Offer from {senderId} to {targetUserId}");
        await Clients.User(targetUserId).SendAsync("ReceiveOffer", offer);
    }

    public async Task SendAnswer(string targetUserId, string answer)
    {
        var senderId = Context.Items["userId"];
        Console.WriteLine($"[SignalR] Sending Answer from {senderId} to {targetUserId}");
        await Clients.User(targetUserId).SendAsync("ReceiveAnswer", answer);
    }

    public async Task SendCandidate(string targetUserId, string candidate)
    {
        var senderId = Context.Items["userId"];
        Console.WriteLine($"[SignalR] Sending Candidate from {senderId} to {targetUserId}");
        await Clients.User(targetUserId).SendAsync("ReceiveCandidate", candidate);
    }

    public async Task SendCallEnded(string targetUserId)
    {
        var senderId = Context.Items["userId"];
        Console.WriteLine($"[SignalR] {senderId} ended call with {targetUserId}");

        // Gửi sự kiện đến người còn lại để họ thoát khỏi màn hình gọi
        await Clients.User(targetUserId).SendAsync("ReceiveCallEnded");
    }


}
