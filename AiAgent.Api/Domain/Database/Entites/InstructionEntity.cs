using MongoDB.Bson.Serialization.Attributes;

namespace AiAgent.Api.Domain.Database.Entites;

public class InstructionEntity : BaseEntity
{
    [BsonElement("key")]
    public string Key { get; set; }

    [BsonElement("module")]
    public string Module { get; set; }

    [BsonElement("content")]
    public string Content { get; set; }
}