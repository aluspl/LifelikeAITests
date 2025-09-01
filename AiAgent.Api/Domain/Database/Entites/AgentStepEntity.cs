using AiAgent.Api.Domain.Chat.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace AiAgent.Api.Domain.Database.Entites
{
    public class AgentStepEntity : BaseEntity
    {
        [BsonElement("order")]
        public int Order { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("instruction")]
        public string Instruction { get; set; }

        [BsonElement("knowledgeIds")]
        public List<Guid> KnowledgeIds { get; set; } = [];

        [BsonElement("modelProvider")]
        public AiProvider ModelProvider { get; set; }

        [BsonElement("isCached")]
        public bool IsCached { get; set; }
    }
}
