namespace AiAgent.Api.Domain.Agents.KillTeam.Models;

public class KillTeamAnalysisResult
{
    public Team Team1 { get; set; }
    public Team Team2 { get; set; }
    public TacticalAnalysis TacticalAnalysis { get; set; }
}
