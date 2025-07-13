using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChatappLC.Infrastructure.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string senderId, string receiverId, string message)
        {
            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, message);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.GetHttpContext()?.Request.Query["userId"].FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(userId))
            {
                Context.Items["userId"] = userId;
            }

            await base.OnConnectedAsync();
        }
    }
}