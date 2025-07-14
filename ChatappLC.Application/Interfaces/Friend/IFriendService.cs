using ChatappLC.Application.DTOs.User;
using ChatappLC.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChatappLC.Application.Interfaces.Friend
{
    public interface IFriendService
    {
        Task<bool> SendFriendRequestAsync(string senderId, string receiverId);
        Task<bool> AcceptFriendRequestAsync(string requestId, string receiverId);
        Task<List<UserResponse>> GetFriendListAsync(string userId);

        Task<List<FriendRequest>> GetPendingRequestsAsync(string userId);
        Task<FriendRequest?> GetByIdAsync(string requestId);
        Task<bool> RemoveFriendAsync(string userId1, string userId2);

    }
}
