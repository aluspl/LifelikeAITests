namespace AiAgent.Api.Domain.Agents.KillTeam.Models;

public class Ploy
{
    public string Name { get; set; }
    public string Type { get; set; } // Strategic or Tactical
    public int CommandPoints { get; set; }
    public string Description { get; set; }
}
