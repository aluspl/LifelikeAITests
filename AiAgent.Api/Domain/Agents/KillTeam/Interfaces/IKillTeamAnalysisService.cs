using AiAgent.Api.Domain.Agents.KillTeam.Models;

namespace AiAgent.Api.Domain.Agents.KillTeam.Interfaces;

public interface IKillTeamAnalysisService
{
    Task<KillTeamAnalysisResult> AnalyzeTeamsAsync(KillTeamAnalysisRequest request);
}
