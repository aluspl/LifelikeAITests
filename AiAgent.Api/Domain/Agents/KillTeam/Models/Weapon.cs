namespace AiAgent.Api.Domain.Agents.KillTeam.Models;

public class Weapon
{
    public string Name { get; set; }
    public string Type { get; set; } // Melee or Ranged
    public int Attacks { get; set; }
    public int BallisticSkill { get; set; } // or Weapon Skill
    public int Damage { get; set; }
    public int CriticalDamage { get; set; }
    public List<SpecialRule> SpecialRules { get; set; } = new();
}
