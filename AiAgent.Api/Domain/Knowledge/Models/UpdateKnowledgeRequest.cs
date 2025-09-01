using AiAgent.Api.Domain.Knowledge.Enums;
using AiAgent.Api.Domain.Knowledge.Models;

namespace AiAgent.Api.Domain.Knowledge.Models;

public class UpdateKnowledgeRequest
{
    public string Key { get; set; }
    public string Module { get; set; }
    public KnowledgeSourceType SourceType { get; set; }
    public List<KnowledgeItem> Items { get; set; } = new List<KnowledgeItem>();
    public string BlobUrl { get; set; }
}