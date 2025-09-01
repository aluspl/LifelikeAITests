namespace AiAgent.Api.Domain.StepCache.Models;

public class StepCacheRequest
{
    public Guid AgentStepId { get; set; }
    public string Query { get; set; }
    public string Value { get; set; }
}
