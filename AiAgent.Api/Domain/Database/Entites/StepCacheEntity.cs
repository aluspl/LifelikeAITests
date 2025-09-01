using MongoDB.Bson.Serialization.Attributes;

namespace AiAgent.Api.Domain.Database.Entites
{
    public class StepCacheEntity : BaseEntity
    {
        [BsonElement("agentStepId")]
        public Guid AgentStepId { get; set; }

        [BsonElement("query")]
        public string Query { get; set; }

        [BsonElement("value")]
        public string Value { get; set; }
    }
}
