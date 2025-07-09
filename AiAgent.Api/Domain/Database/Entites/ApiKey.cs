using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AiAgent.Api.Domain.Database.Entites;

public class ApiKey : BaseEntity
{
    [BsonElement("key")]
    public string Key { get; set; } = string.Empty;

    [BsonElement("owner")]
    public string Owner { get; set; } = string.Empty;

    [BsonElement("created")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Created { get; set; } = DateTime.UtcNow;

    [BsonElement("expires")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? Expires { get; set; }
}
