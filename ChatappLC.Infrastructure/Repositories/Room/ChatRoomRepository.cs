namespace ChatappLC.Infrastructure.Repositories.Room;

internal class ChatRoomRepository : IChatRoomRepository
{
    private readonly IMongoCollection<ChatRoom> _chatRooms;

    public ChatRoomRepository(MongoDbContext mongoDbContext)
    {
        _chatRooms = mongoDbContext.Database.GetCollection<ChatRoom>("ChatRooms");
    }

    public async Task<ChatRoom> CreateChatRoomAsync(ChatRoom chatRoom)
    {
        await _chatRooms.InsertOneAsync(chatRoom);
        return chatRoom;
    }

    public async Task<ChatRoom> GetChatRoomByIdAsync(string id)
    {
        ObjectId.TryParse(id, out var objectId);
        var filter = Builders<ChatRoom>.Filter.Eq(c => c.Id, objectId);
        return await _chatRooms.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<ChatRoom>> GetChatRoomsByUserIdAsync(string userId)
    {
        var filter = Builders<ChatRoom>.Filter.AnyEq(c => c.ParticipantIds, userId);
        var chatRooms = await _chatRooms.Find(filter).ToListAsync();

        // Filter out chat rooms with null Ids and sort by LastMessageTime in ascending order
        return chatRooms
            .Where(c => c.Id != ObjectId.Empty)
            .OrderByDescending(c => c.LastMessageTime);
    }


    public async Task<ChatRoom> GetChatRoomByUserOneAndOne(string userIdOne, string userIdTwo)
    {
        var filter = Builders<ChatRoom>.Filter.And(
            Builders<ChatRoom>.Filter.All(c => c.ParticipantIds, new List<string> { userIdOne, userIdTwo }),
            Builders<ChatRoom>.Filter.Size(c => c.ParticipantIds, 2), // Ensure there are only two participants
            Builders<ChatRoom>.Filter.Eq(c => c.IsGroup, false) // Ensure it's not a group chat
        );
        return await _chatRooms.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateChatRoomAsync(ChatRoom chatRoom)
    {
        var filter = Builders<ChatRoom>.Filter.Eq(c => c.Id, chatRoom.Id);
        if (filter == null)
        {
            return false;
        }
        await _chatRooms.ReplaceOneAsync(filter, chatRoom);
        return true;
    }

    //method này để gọi chạm nhẹ để mở kết nối mongo trong lần đầu khởi tạo
    public async Task WarmupAsync()
    {
        var filter = Builders<ChatRoom>.Filter.Empty;
        await _chatRooms.Find(filter).FirstOrDefaultAsync();
    }

}
