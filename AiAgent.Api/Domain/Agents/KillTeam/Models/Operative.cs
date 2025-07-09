namespace AiAgent.Api.Domain.Agents.KillTeam.Models;

public class Operative
{
    public string Name { get; set; }
    public int Movement { get; set; }
    public int ActionPointLimit { get; set; }
    public int GroupActivation { get; set; }
    public int Defence { get; set; }
    public int Save { get; set; }
    public int Wounds { get; set; }
    public List<string> Abilities { get; set; } = new();
    public List<Weapon> Weapons { get; set; } = new();
}
