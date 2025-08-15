using System.Threading.Tasks;

namespace AiAgent.Api.Domain.Agents.Orchestration
{
    public interface IAgentOrchestratorService
    {
        Task<string> ExecuteAgent(string agentId, string initialInput);
    }
}
