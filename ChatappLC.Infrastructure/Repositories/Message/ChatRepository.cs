namespace ChatappLC.Infrastructure.Repositories.Message;

internal class ChatRepository : IChatRepository
{
    private readonly IMongoCollection<ChatMessage> _chatMessages;

    public ChatRepository(MongoDbContext mongoDbContext)
    {
        _chatMessages = mongoDbContext.Database.GetCollection<ChatMessage>("ChatMessages");
    }

    public async Task<ChatMessage> CreateChatMessageAsync(ChatMessage message)
    {
        await _chatMessages.InsertOneAsync(message);
        return message;
    }

    public async Task<ChatMessage> GetChatMessageByIdAsync(string id)
    {
        ObjectId.TryParse(id, out var objectId);
        var filter = Builders<ChatMessage>.Filter.Eq(m => m.Id, objectId);
        return await _chatMessages.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ChatMessage>> GetChatMessagesByRoomIdAsync(string chatRoomId)
    {
        var filter = Builders<ChatMessage>.Filter.Eq(m => m.ChatRoomId, chatRoomId);
        return await _chatMessages.Find(filter)
                                  .SortBy(m => m.Timestamp)
                                  .ToListAsync();
    }

    public async Task UpdateMessageAsync(ChatMessage message)
    {
        var filter = Builders<ChatMessage>.Filter.Eq(m => m.Id, message.Id);
        await _chatMessages.ReplaceOneAsync(filter, message);
    }

    public async Task DeleteMessageAsync(string id)
    {
        ObjectId.TryParse(id, out var objectId);
        var filter = Builders<ChatMessage>.Filter.Eq(m => m.Id, objectId);
        await _chatMessages.DeleteOneAsync(filter);
    }

}
