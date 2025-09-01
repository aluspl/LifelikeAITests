namespace AiAgent.Api.Domain.StepCache.Models;

public class StepCacheResponse
{
    public Guid Id { get; set; }
    public int AgentStepId { get; set; }
    public string Query { get; set; }
    public string Value { get; set; }
}