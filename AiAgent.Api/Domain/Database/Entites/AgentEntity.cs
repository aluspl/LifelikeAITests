using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using AiAgent.Api.Domain.Database.Entites;

namespace AiAgent.Api.Domain.Database.Entites
{
    public class AgentEntity : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        [BsonElement("stepIds")]
        public List<string> StepIds { get; set; } = new List<string>();
    }
}
