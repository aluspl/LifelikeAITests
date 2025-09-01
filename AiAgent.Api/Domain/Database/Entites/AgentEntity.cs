using MongoDB.Bson.Serialization.Attributes;

namespace AiAgent.Api.Domain.Database.Entites
{
    public class AgentEntity : BaseEntity
    {
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("description")]
        public string Description { get; set; }
        [BsonElement("stepIds")]
        public List<Guid> StepIds { get; set; } = [];
    }
}
