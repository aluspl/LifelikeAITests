namespace AiAgent.Api.Domain.Agents.KillTeam.Models;

public class KillTeamAnalysisRequest
{
    public string Team1Name { get; set; }
    public string Team2Name { get; set; }
    public string Format { get; set; } = "json"; // "json" or "markdown"
    public string Language { get; set; } = "en";
}
