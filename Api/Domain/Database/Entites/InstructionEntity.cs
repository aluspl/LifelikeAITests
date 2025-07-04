using MongoDB.Bson.Serialization.Attributes;

namespace Api.Domain.Database.Entites;

public class InstructionEntity : BaseEntity
{
    [BsonElement("module")]
    public string Module { get; set; }

    [BsonElement("content")]
    public string Content { get; set; }
}
