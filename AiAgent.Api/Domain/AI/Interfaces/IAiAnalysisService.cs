using AiAgent.Api.Domain.Agents.KillTeam.Models;

namespace AiAgent.Api.Domain.AI.Interfaces;

public interface IAiAnalysisService
{
    Task<KillTeamAnalysisResult> AnalyzeKillTeamsAsync(string team1Name, string team2Name, string language);
}
