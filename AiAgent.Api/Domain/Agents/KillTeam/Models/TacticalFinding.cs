namespace AiAgent.Api.Domain.Agents.KillTeam.Models;

public class TacticalFinding
{
    public string FindingType { get; set; } // e.g., "Armor Penetration", "Conceal Counter"
    public string Description { get; set; }
    public string TeamAdvantage { get; set; } // The team that has the advantage
    public List<string> RelevantAssets { get; set; } = new(); // e.g., names of weapons, operatives, or ploys
}
