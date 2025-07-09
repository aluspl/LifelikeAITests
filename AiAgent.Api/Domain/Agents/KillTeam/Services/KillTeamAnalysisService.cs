using System.Text.Json;
using AiAgent.Api.Domain.Agents.KillTeam.Interfaces;
using AiAgent.Api.Domain.Agents.KillTeam.Models;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Common.Enums;
using System.Text.RegularExpressions;

namespace AiAgent.Api.Domain.Agents.KillTeam.Services;

public class KillTeamAnalysisService : IKillTeamAnalysisService
{
    private readonly IKnowledgeRepository _knowledgeRepository;
    private readonly IInstructionRepository _instructionRepository;

    public KillTeamAnalysisService(IKnowledgeRepository knowledgeRepository, IInstructionRepository instructionRepository)
    {
        _knowledgeRepository = knowledgeRepository;
        _instructionRepository = instructionRepository;
    }

    public async Task<KillTeamAnalysisResult> AnalyzeTeamsAsync(KillTeamAnalysisRequest request)
    {
        // Step 1: (Implicit) Instructions could be loaded here if they influence the logic.
        // var instructions = await GetAgentInstructionsAsync("KillTeamAgent");

        // Step 2: Retrieve and build the internal working object
        var team1Data = await RetrieveTeamDataAsync(request.Team1Name);
        var team2Data = await RetrieveTeamDataAsync(request.Team2Name);

        // Step 3: Validate the data
        ValidateRetrievedData(team1Data, team2Data);

        var result = new KillTeamAnalysisResult
        {
            Team1 = team1Data,
            Team2 = team2Data
        };

        // Step 4: Perform Tactical Analysis
        result.TacticalAnalysis = PerformTacticalAnalysis(result.Team1, result.Team2);

        // Step 5: Translation (if needed)
        if (request.Language != "en")
        {
            await TranslateResultsAsync(result, request.Language);
        }

        // Step 6: Final formatting is handled by the serialization in the controller for JSON.
        // Markdown would require a specific formatter here.

        return result;
    }

    private async Task<Team> RetrieveTeamDataAsync(string teamName)
    {
        // 1. Get Team Composition
        var compositionKey = $"Kill Team Composition: {teamName}";
        Console.WriteLine($"Attempting to retrieve composition for key: {compositionKey}, module: {ModuleType.KillTeam}");
        var compositionEntity = await _knowledgeRepository.GetByKeyAndModuleAsync(compositionKey, ModuleType.KillTeam.ToString());
        if (compositionEntity == null)
        {
            Console.WriteLine($"Composition entity is null for key: {compositionKey}");
            throw new ArgumentException($"Team composition not found for '{teamName}'.");
        }

        var jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var composition = ParseTeamComposition(compositionEntity.Value);
        if (composition == null)
            throw new InvalidDataException($"Could not parse team composition for '{teamName}'.");

        // 3. Fetch all details in bulk
        var detailKeys = new List<string>();
        detailKeys.AddRange(composition.Operatives.Select(o => $"Operative: {o}"));
        detailKeys.AddRange(composition.Ploys.Select(p => $"Ploy: {p}"));
        detailKeys.AddRange(composition.Equipment.Select(e => $"Equipment: {e}"));

        var detailEntities = await _knowledgeRepository.GetByKeysAndModuleAsync(detailKeys, ModuleType.KillTeam.ToString());
        var detailsMap = detailEntities.ToDictionary(e => e.Key, e => e.Value);

        // 4. Build the Team object
        var team = new Team { TeamName = teamName };

        team.Operatives = composition.Operatives
            .Select(name => JsonSerializer.Deserialize<Operative>(detailsMap[$"Operative: {name}"], jsonOptions))
            .ToList();
        
        team.Ploys = composition.Ploys
            .Select(name => JsonSerializer.Deserialize<Ploy>(detailsMap[$"Ploy: {name}"], jsonOptions))
            .ToList();

        team.Equipment = composition.Equipment
            .Select(name => JsonSerializer.Deserialize<Equipment>(detailsMap[$"Equipment: {name}"], jsonOptions))
            .ToList();

        // 5. Discover, fetch, and embed weapon special rule descriptions
        var allRuleNames = team.Operatives
            .SelectMany(o => o.Weapons)
            .SelectMany(w => w.SpecialRules.Select(r => r.Name))
            .Distinct()
            .ToList();

        if (allRuleNames.Any())
        {
            var ruleKeys = allRuleNames.Select(r => $"Weapon Rule: {r}").ToList();
            var ruleEntities = await _knowledgeRepository.GetByKeysAndModuleAsync(ruleKeys, ModuleType.KillTeam.ToString());
            var rulesMap = ruleEntities.ToDictionary(
                e => e.Key.Replace("Weapon Rule: ", ""),
                e => JsonSerializer.Deserialize<SpecialRule>(e.Value, jsonOptions)
            );

            // Embed the descriptions
            foreach (var operative in team.Operatives)
            {
                foreach (var weapon in operative.Weapons)
                {
                    for (int i = 0; i < weapon.SpecialRules.Count; i++)
                    {
                        if (rulesMap.TryGetValue(weapon.SpecialRules[i].Name, out var fullRule))
                        {
                            weapon.SpecialRules[i] = fullRule;
                        }
                    }
                }
            }
        }

        return team;
    }

    private class TeamCompositionDto
    {
        public List<string> Operatives { get; set; } = new();
        public List<string> Ploys { get; set; } = new();
        public List<string> Equipment { get; set; } = new();
    }

    private TeamCompositionDto ParseTeamComposition(string rawComposition)
    {
        var composition = new TeamCompositionDto();

        // Extract Operatives
        var operativeMatch = Regex.Match(rawComposition, @"Operatives:\s*(.*?)(?:Ploys:|Equipments:|Faction rule:|Operative rule:|Kill Team Composition:|$)", RegexOptions.Singleline);
        if (operativeMatch.Success)
        {
            var operativesText = operativeMatch.Groups[1].Value;
            var operativeNames = Regex.Matches(operativesText, @"\b([A-Za-z\s-]+?)(?:,\s*|\.|\s*$)")
                                    .Cast<Match>()
                                    .Select(m => m.Groups[1].Value.Trim())
                                    .Where(s => !string.IsNullOrEmpty(s) && !s.Contains("Operatives:")) // Filter out "Operatives:" itself
                                    .ToList();
            composition.Operatives.AddRange(operativeNames);
        }

        // Extract Ploys
        var ploysMatch = Regex.Match(rawComposition, @"Ploys:\s*(.*?)(?:Equipments:|Faction rule:|Operative rule:|Kill Team Composition:|$)", RegexOptions.Singleline);
        if (ploysMatch.Success)
        {
            var ploysText = ploysMatch.Groups[1].Value;
            var ployNames = Regex.Matches(ploysText, @"\b([A-Za-z\s-]+?)(?:,\s*|\.|\s*$)")
                                .Cast<Match>()
                                .Select(m => m.Groups[1].Value.Trim())
                                .Where(s => !string.IsNullOrEmpty(s) && !s.Contains("Ploys:")) // Filter out "Ploys:" itself
                                .ToList();
            composition.Ploys.AddRange(ployNames);
        }

        // Extract Equipment
        var equipmentMatch = Regex.Match(rawComposition, @"Equipments:\s*(.*?)(?:Faction rule:|Operative rule:|Kill Team Composition:|$)", RegexOptions.Singleline);
        if (equipmentMatch.Success)
        {
            var equipmentText = equipmentMatch.Groups[1].Value;
            var equipmentNames = Regex.Matches(equipmentText, @"\b([A-Za-z\s-]+?)(?:,\s*|\.|\s*$)")
                                    .Cast<Match>()
                                    .Select(m => m.Groups[1].Value.Trim())
                                    .Where(s => !string.IsNullOrEmpty(s) && !s.Contains("Equipments:")) // Filter out "Equipments:" itself
                                    .ToList();
            composition.Equipment.AddRange(equipmentNames);
        }

        return composition;
    }

    private void ValidateRetrievedData(Team team1, Team team2)
    {
        if (team1 == null || !team1.Operatives.Any())
            throw new InvalidDataException($"Data validation failed: Team 1 (\"{team1?.TeamName}\") has no operatives or could not be loaded.");

        if (team2 == null || !team2.Operatives.Any())
            throw new InvalidDataException($"Data validation failed: Team 2 (\"{team2?.TeamName}\") has no operatives or could not be loaded.");

        // A simple check to ensure operatives have key stats, implying successful deserialization.
        if (team1.Operatives.Any(o => o.Wounds <= 0) || team2.Operatives.Any(o => o.Wounds <= 0))
        {
            throw new InvalidDataException("Data validation failed: Some operatives have invalid stats (e.g., Wounds <= 0).");
        }
    }

    private TacticalAnalysis PerformTacticalAnalysis(Team teamA, Team teamB)
    {
        var analysis = new TacticalAnalysis
        {
            Findings = new List<TacticalFinding>()
        };

        // Run analysis for Team A vs Team B
        TestArmorPenetration(teamA, teamB, analysis);
        TestConcealCounters(teamA, teamB, analysis);
        TestWoundEndurance(teamA, teamB, analysis);
        TestMeleeCounters(teamA, teamB, analysis);
        TestSpecialDefenses(teamA, teamB, analysis);

        // Run analysis for Team B vs Team A
        TestArmorPenetration(teamB, teamA, analysis);
        TestConcealCounters(teamB, teamA, analysis);
        TestWoundEndurance(teamB, teamA, analysis);
        TestMeleeCounters(teamB, teamA, analysis);
        TestSpecialDefenses(teamB, teamA, analysis);

        if (!analysis.Findings.Any())
        {
            analysis.Summary = "No specific tactical advantages or disadvantages were identified based on the available data.";
        }
        else
        {
            analysis.Summary = $"Tactical analysis complete for {teamA.TeamName} vs {teamB.TeamName}. Found {analysis.Findings.Count} key interactions.";
        }

        return analysis;
    }

    private void TestArmorPenetration(Team attackingTeam, Team defendingTeam, TacticalAnalysis analysis)
    {
        const int HighSaveThreshold = 4; // Corresponds to 4+ or better

        var highSaveOperatives = defendingTeam.Operatives.Where(o => o.Save <= HighSaveThreshold).ToList();

        if (!highSaveOperatives.Any()) return;

        var counterWeapons = attackingTeam.Operatives
            .SelectMany(o => o.Weapons)
            .Where(w => w.SpecialRules.Any(r => r.Name.StartsWith("AP") || r.Name.StartsWith("P")))
            .ToList();

        if (!counterWeapons.Any()) return;

        var finding = new TacticalFinding
        {
            FindingType = "Armor Penetration Advantage",
            TeamAdvantage = attackingTeam.TeamName,
            Description = $"{attackingTeam.TeamName} has weapons with armor piercing (AP/P), which are effective against the high armor of {defendingTeam.TeamName}'s operatives (e.g., {string.Join(", ", highSaveOperatives.Select(o => o.Name).Take(3))}).",
            RelevantAssets = counterWeapons.Select(w => w.Name).Distinct().ToList()
        };

        analysis.Findings.Add(finding);
    }

    private void TestConcealCounters(Team attackingTeam, Team defendingTeam, TacticalAnalysis analysis)
    {
        var concealOperatives = defendingTeam.Operatives
            .Where(o => o.Abilities.Any(a => a.ToLower().Contains("conceal")))
            .ToList();

        if (!concealOperatives.Any()) return;

        var counterAssets = attackingTeam.Operatives
            .SelectMany(o => o.Weapons.Select(w => w.Name))
            .Concat(attackingTeam.Equipment.Select(e => e.Name))
            .Where(a => a.ToLower().Contains("auspex") || a.ToLower().Contains("indirect") || a.ToLower().Contains("no cover"))
            .Distinct()
            .ToList();

        if (!counterAssets.Any()) return;

        analysis.Findings.Add(new TacticalFinding
        {
            FindingType = "Concealment Counter",
            TeamAdvantage = attackingTeam.TeamName,
            Description = $"{attackingTeam.TeamName} possesses assets (e.g., {string.Join(", ", counterAssets)}) that can ignore or mitigate the Conceal-based strategies of {defendingTeam.TeamName}.",
            RelevantAssets = counterAssets
        });
    }

    private void TestWoundEndurance(Team attackingTeam, Team defendingTeam, TacticalAnalysis analysis)
    {
        const int HighWoundsThreshold = 12;
        if (defendingTeam.Operatives.All(o => o.Wounds < HighWoundsThreshold)) return;

        var highDamageWeapons = attackingTeam.Operatives
            .SelectMany(o => o.Weapons)
            .Where(w => w.Damage >= 4 || w.SpecialRules.Any(r => r.Name.StartsWith("MW")))
            .ToList();

        if (!highDamageWeapons.Any()) return;

        analysis.Findings.Add(new TacticalFinding
        {
            FindingType = "High Damage Advantage",
            TeamAdvantage = attackingTeam.TeamName,
            Description = $"{attackingTeam.TeamName} has high-damage or mortal wound-inflicting weapons, effective against the high-wound operatives of {defendingTeam.TeamName}.",
            RelevantAssets = highDamageWeapons.Select(w => w.Name).Distinct().ToList()
        });
    }

    private void TestMeleeCounters(Team attackingTeam, Team defendingTeam, TacticalAnalysis analysis)
    {
        var isMeleeHeavy = defendingTeam.Operatives.SelectMany(o => o.Weapons).Count(w => w.Type == "Melee") > defendingTeam.Operatives.Count;
        if (!isMeleeHeavy) return;

        var counterPloys = attackingTeam.Ploys
            .Where(p => p.Description.ToLower().Contains("fall back") || p.Description.ToLower().Contains("shoot in engagement"))
            .ToList();

        if (!counterPloys.Any()) return;

        analysis.Findings.Add(new TacticalFinding
        {
            FindingType = "Melee Counter-Play",
            TeamAdvantage = attackingTeam.TeamName,
            Description = $"{attackingTeam.TeamName} has ploys that provide effective counter-play against the melee-heavy composition of {defendingTeam.TeamName}.",
            RelevantAssets = counterPloys.Select(p => p.Name).ToList()
        });
    }

    private void TestSpecialDefenses(Team attackingTeam, Team defendingTeam, TacticalAnalysis analysis)
    {
        var specialDefenseOperatives = defendingTeam.Operatives
            .Where(o => o.Abilities.Any(a => a.ToLower().Contains("feel no pain") || a.ToLower().Contains("ignore damage")))
            .ToList();

        if (!specialDefenseOperatives.Any()) return;

        var counterWeapons = attackingTeam.Operatives
            .SelectMany(o => o.Weapons)
            .Where(w => w.Attacks >= 4 || w.SpecialRules.Any(r => r.Name.Equals("Relentless", StringComparison.OrdinalIgnoreCase)))
            .ToList();

        if (!counterWeapons.Any()) return;

        analysis.Findings.Add(new TacticalFinding
        {
            FindingType = "Defense Bypass",
            TeamAdvantage = attackingTeam.TeamName,
            Description = $"{attackingTeam.TeamName} fields weapons with a high volume of attacks or the 'Relentless' rule, which are effective at bypassing the special defenses of {defendingTeam.TeamName}.",
            RelevantAssets = counterWeapons.Select(w => w.Name).Distinct().ToList()
        });
    }

    private async Task TranslateResultsAsync(KillTeamAnalysisResult result, string language)
    {
        // In a real application, this would integrate with a translation service.
        // For now, it's a placeholder.
        Console.WriteLine($"Translation to {language} is not implemented. Results will be in English.");
        await Task.CompletedTask;
    }
}