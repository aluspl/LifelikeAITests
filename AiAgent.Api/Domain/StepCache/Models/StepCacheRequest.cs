namespace AiAgent.Api.Domain.StepCache.Models;

public class InstructionRequest
{
    public Guid AgentId { get; set; }
    public string Query { get; set; }
    public string Content { get; set; }
}
