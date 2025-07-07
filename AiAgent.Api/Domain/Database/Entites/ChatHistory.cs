using MongoDB.Bson.Serialization.Attributes;

namespace AiAgent.Api.Domain.Database.Entites;

public class ChatHistoryEntity : BaseEntity
{
    [BsonElement("Query")]
    public string Query { get; set; }

    [BsonElement("Response")]
    public string Response { get; set; }

    [BsonElement("Model")]
    public string Model { get; set; }

    [BsonElement("Role")]
    public string Role { get; set; }
}