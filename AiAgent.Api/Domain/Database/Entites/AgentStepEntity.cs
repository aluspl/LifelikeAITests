using AiAgent.Api.Domain.Chat.Enums;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace AiAgent.Api.Domain.Database.Entites
{
    public class AgentStepEntity : BaseEntity
    {
        [BsonElement("order")]
        public int Order { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("instructionId")]
        public string InstructionId { get; set; }

        [BsonElement("knowledgeIds")]
        public List<string> KnowledgeIds { get; set; } = new List<string>();

        [BsonElement("modelProvider")]
        public AiProvider ModelProvider { get; set; }

        [BsonElement("isCached")]
        public bool IsCached { get; set; }
    }
}
