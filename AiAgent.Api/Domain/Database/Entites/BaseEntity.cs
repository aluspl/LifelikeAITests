using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AiAgent.Api.Domain.Database.Entites;

public class BaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonElement("key")] // Restored
    public string Key { get; set; } // Restored

    [BsonElement("Created")]
    public DateTime Created { get; set; }

    [BsonElement("Updated")]
    public DateTime? Updated { get; set; }
}