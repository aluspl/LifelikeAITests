using AiAgent.Api.Domain.Common.Interfaces;

namespace AiAgent.Api.Domain.Agents.Orchestration.Interfaces;

public interface IAgentOrchestratorService : IService
{
    Task<string> ExecuteAgent(Guid agentId, string initialInput);
}
