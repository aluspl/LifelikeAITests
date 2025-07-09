namespace AiAgent.Api.Domain.Agents.KillTeam.Models;

public class Team
{
    public string TeamName { get; set; }
    public List<Operative> Operatives { get; set; } = new();
    public List<Ploy> Ploys { get; set; } = new();
    public List<Equipment> Equipment { get; set; } = new();
}
