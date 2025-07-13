using ChatappLC.Application.DTOs.User;
using ChatappLC.Application.Interfaces.Friend;
using ChatappLC.Domain.Entities;
using ChatappLC.Infrastructure.MongoDb;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatappLC.Infrastructure.Services.Friend
{
    public class FriendService : IFriendService
    {
        private readonly IMongoCollection<FriendRequest> _friendRequests;
        private readonly IMongoCollection<User> _users;

        public FriendService(MongoDbContext context)
        {
            _friendRequests = context.Friends;
            _users = context.Users;
        }

        public async Task<bool> SendFriendRequestAsync(string senderId, string receiverId)
        {
            var existing = await _friendRequests.Find(x =>
                (x.SenderId == senderId && x.ReceiverId == receiverId) ||
                (x.SenderId == receiverId && x.ReceiverId == senderId))
                .FirstOrDefaultAsync();

            if (existing != null) return false;

            var request = new FriendRequest
            {
                SenderId = senderId,
                ReceiverId = receiverId
            };

            await _friendRequests.InsertOneAsync(request);
            return true;
        }

        public async Task<bool> AcceptFriendRequestAsync(string requestId, string receiverId)
        {
            var request = await _friendRequests.Find(x => x.Id == requestId && x.ReceiverId == receiverId).FirstOrDefaultAsync();
            if (request == null || request.IsAccepted) return false;

            request.IsAccepted = true;
            await _friendRequests.ReplaceOneAsync(x => x.Id == requestId, request);
            return true;
        }

        public async Task<List<UserResponse>> GetFriendListAsync(string userId)
        {
            var accepted = await _friendRequests.Find(x =>
                x.IsAccepted && (x.SenderId == userId || x.ReceiverId == userId))
                .ToListAsync();

            var friendIds = accepted
                .Select(x => x.SenderId == userId ? x.ReceiverId : x.SenderId)
                .Distinct()
                .ToList();

            var filter = Builders<User>.Filter.In(u => u.Id, friendIds);
            var friendUsers = await _users.Find(filter).ToListAsync();

            return friendUsers.Select(u => new UserResponse
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber
            }).ToList();
        }

        public async Task<List<FriendRequest>> GetPendingRequestsAsync(string userId)
        {
            return await _friendRequests.Find(x => x.ReceiverId == userId && !x.IsAccepted).ToListAsync();
        }

        public async Task<FriendRequest?> GetByIdAsync(string requestId)
        {
            return await _friendRequests
                .Find(r => r.Id == requestId)
                .FirstOrDefaultAsync();
        }
    }
}
