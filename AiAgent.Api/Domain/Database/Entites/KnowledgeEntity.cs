using MongoDB.Bson.Serialization.Attributes;

namespace AiAgent.Api.Domain.Database.Entites;

[BsonIgnoreExtraElements]
public class KnowledgeEntity : BaseEntity
{
    [BsonElement("Key")]
    public string Key { get; set; }

    [BsonElement("Value")]
    public string Value { get; set; }

    [BsonElement("Module")]
    public string Module { get; set; } = string.Empty;
}
