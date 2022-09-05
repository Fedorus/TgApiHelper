using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TelegramFaqBotHost.MongoModels
{
    public class MongoItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}