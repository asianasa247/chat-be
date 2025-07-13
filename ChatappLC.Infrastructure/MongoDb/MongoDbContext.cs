using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ChatappLC.Domain.Entities;

namespace ChatappLC.Infrastructure.MongoDb
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoDatabase Database => _database;
        public IMongoCollection<User> Users => _database.GetCollection<User>("Users");
        public IMongoCollection<Message> Messages => _database.GetCollection<Message>("Messages");
        public IMongoCollection<FriendRequest> Friends => _database.GetCollection<FriendRequest>("Friends");
        public IMongoCollection<RefreshToken> RefreshTokens => _database.GetCollection<RefreshToken>("RefreshTokens");
    }
}