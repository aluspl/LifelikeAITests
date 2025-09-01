using MongoDB.Bson.Serialization.Attributes;
using AiAgent.Api.Domain.Knowledge.Enums;
using AiAgent.Api.Domain.Knowledge.Models;

namespace AiAgent.Api.Domain.Database.Entites
{
    [BsonIgnoreExtraElements]
    public class KnowledgeEntity : BaseEntity
    {
        [BsonElement("key")]
        public string Key { get; set; }

        [BsonElement("module")]
        public string Module { get; set; } = string.Empty;

        [BsonElement("sourceType")]
        public KnowledgeSourceType SourceType { get; set; }

        [BsonElement("items")]
        public List<KnowledgeItem> Items { get; set; } = new List<KnowledgeItem>();

        [BsonElement("blobUrl")]
        public string BlobUrl { get; set; }
    }
}