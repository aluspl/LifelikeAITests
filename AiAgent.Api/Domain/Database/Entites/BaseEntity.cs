using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AiAgent.Api.Domain.Database.Entites;

public class BaseEntity
{
    [BsonId]
    public Guid Id { get; set; }

    [BsonElement("Created")]
    public DateTime Created { get; set; }

    [BsonElement("Updated")]
    public DateTime? Updated { get; set; }
}