namespace AiAgent.Api.Domain.Agents.KillTeam.Models;

public class TacticalAnalysis
{
    public string Summary { get; set; }
    public List<TacticalFinding> Findings { get; set; } = new();
}
