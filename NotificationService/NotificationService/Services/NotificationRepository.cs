using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using NotificationService.Models;

namespace NotificationService.Services
{
    public class NotificationRepository
    {
        private readonly IMongoCollection<NotificationEntry> _collection;

        public NotificationRepository(IOptions<MongoDbSettings> settings)
        {


            var mongoClient = new MongoClient(settings.Value.ConnectionString);
            var database = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _collection = database.GetCollection<NotificationEntry>(settings.Value.CollectionName);
        }
        public async Task AddAsync(NotificationEntry entry)
        {
            await _collection.InsertOneAsync(entry);
        }
    }
}
