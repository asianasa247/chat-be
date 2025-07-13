using ChatappLC.Application.Interfaces.Messages;
using ChatappLC.Domain.Entities;
using ChatappLC.Infrastructure.Hubs;
using ChatappLC.Infrastructure.MongoDb;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace ChatappLC.Infrastructure.Services.Messages
{
    public class MessageService : IMessageService
    {
        private readonly IMongoCollection<Message> _messages;
        private readonly IHubContext<ChatHub> _hubContext;

        public MessageService(MongoDbContext context, IHubContext<ChatHub> hubContext)
        {
            _messages = context.Database.GetCollection<Message>("Messages");
            _hubContext = hubContext;
        }

        public async Task<Message> SendMessageAsync(Message message)
        {
            await _messages.InsertOneAsync(message);

            // Gửi realtime tới người gửi và người nhận
            await _hubContext.Clients.User(message.ReceiverId!).SendAsync("MessageSent", message);
            await _hubContext.Clients.User(message.SenderId).SendAsync("MessageSent", message);

            return message;
        }

        public async Task<Message> EditMessageAsync(string messageId, string userId, string newContent)
        {
            var filter = Builders<Message>.Filter.Eq(m => m.Id, messageId);
            var message = await _messages.Find(filter).FirstOrDefaultAsync();

            if (message == null || message.SenderId != userId)
                throw new UnauthorizedAccessException();

            var update = Builders<Message>.Update
                .Set(m => m.Content, newContent)
                .Set(m => m.IsEdited, true);

            await _messages.UpdateOneAsync(filter, update);

            message.Content = newContent;
            message.IsEdited = true;

            // Gửi realtime cập nhật
            await _hubContext.Clients.User(message.SenderId).SendAsync("MessageEdited", message);
            await _hubContext.Clients.User(message.ReceiverId!).SendAsync("MessageEdited", message);

            return message;
        }

        public async Task<Message?> DeleteMessageForEveryoneAsync(string messageId, string userId)
        {
            var message = await _messages.Find(m => m.Id == messageId).FirstOrDefaultAsync();

            if (message == null || message.SenderId != userId)
                return null;

            var update = Builders<Message>.Update.Set(m => m.IsDeletedForEveryone, true);
            await _messages.UpdateOneAsync(m => m.Id == messageId, update);

            message.IsDeletedForEveryone = true;
            return message;
        }

        public async Task<Message?> DeleteMessageForUserAsync(string messageId, string userId)
        {
            var message = await _messages.Find(m => m.Id == messageId).FirstOrDefaultAsync();

            if (message == null)
                return null;

            var update = Builders<Message>.Update.AddToSet(m => m.DeletedForUserIds, userId);
            await _messages.UpdateOneAsync(m => m.Id == messageId, update);

            message.DeletedForUserIds.Add(userId);
            return message;
        }


        public async Task<List<Message>> GetMessagesAsync(string userId, string? receiverId = null, string? groupId = null)
        {
            var filter = Builders<Message>.Filter.Eq(m => m.IsDeletedForEveryone, false) &
                         !Builders<Message>.Filter.AnyEq(m => m.DeletedForUserIds, userId);

            if (!string.IsNullOrEmpty(groupId))
            {
                filter &= Builders<Message>.Filter.Eq(m => m.GroupId, groupId);
            }
            else if (!string.IsNullOrEmpty(receiverId))
            {
                filter &= Builders<Message>.Filter.Or(
                    Builders<Message>.Filter.And(
                        Builders<Message>.Filter.Eq(m => m.SenderId, userId),
                        Builders<Message>.Filter.Eq(m => m.ReceiverId, receiverId)
                    ),
                    Builders<Message>.Filter.And(
                        Builders<Message>.Filter.Eq(m => m.SenderId, receiverId),
                        Builders<Message>.Filter.Eq(m => m.ReceiverId, userId)
                    )
                );
            }

            return await _messages.Find(filter).SortBy(m => m.SentAt).ToListAsync();
        }
        public async Task<List<Message>> GetMessageHistoryForUserAsync(string userId)
        {
            var filterBuilder = Builders<Message>.Filter;

            var filter = filterBuilder.And(
                filterBuilder.Or(
                    filterBuilder.Eq(m => m.SenderId, userId),
                    filterBuilder.Eq(m => m.ReceiverId, userId)
                ),
                !filterBuilder.AnyEq(m => m.DeletedForUserIds, userId)
            );

            var messages = await _messages.Find(filter)
                                          .SortBy(m => m.SentAt)
                                          .ToListAsync();

            return messages;
        }
    }
}
