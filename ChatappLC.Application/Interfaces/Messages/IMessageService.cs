using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatappLC.Domain.Entities;

namespace ChatappLC.Application.Interfaces.Messages
{
    public interface IMessageService
    {
        Task<Message> SendMessageAsync(Message message);
        Task<Message> EditMessageAsync(string messageId, string userId, string newContent);

        Task<Message?> DeleteMessageForEveryoneAsync(string messageId, string userId);
        Task<Message?> DeleteMessageForUserAsync(string messageId, string userId);

        Task<List<Message>> GetMessagesAsync(string userId, string? receiverId = null, string? groupId = null);
        Task<List<Message>> GetMessageHistoryForUserAsync(string userId);

    }
}
