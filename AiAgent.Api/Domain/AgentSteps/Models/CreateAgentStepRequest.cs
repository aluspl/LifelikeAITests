using AiAgent.Api.Domain.Chat.Enums;

namespace AiAgent.Api.Domain.AgentSteps.Models;

public class CreateAgentStepRequest
{
    public int Order { get; set; }
    public string Name { get; set; }
    public string Instruction { get; set; }
    public List<Guid> KnowledgeIds { get; set; } = [];
    public AiProvider ModelProvider { get; set; }
    public bool IsCached { get; set; }
}