using AiAgent.Api.Domain.AI.Interfaces;
using AiAgent.Api.Domain.Agents.KillTeam.Interfaces;
using AiAgent.Api.Domain.Agents.KillTeam.Models;

namespace AiAgent.Api.Domain.AI.Services;

public class AiAnalysisService : IAiAnalysisService
{
    private readonly IKillTeamAnalysisService _killTeamAnalysisService;
    private readonly ILogger<AiAnalysisService> _logger;

    public AiAnalysisService(IKillTeamAnalysisService killTeamAnalysisService, ILogger<AiAnalysisService> logger)
    {
        _killTeamAnalysisService = killTeamAnalysisService;
        _logger = logger;
    }

    public async Task<KillTeamAnalysisResult> AnalyzeKillTeamsAsync(string team1Name, string team2Name, string language)
    {
        _logger.LogInformation($"AI Analysis Service: Analyzing {team1Name} vs {team2Name} in {language}.");

        // Placeholder for LLM interaction to generate data extraction instructions
        _logger.LogInformation("LLM: Generating data extraction instructions...");
        // In a real scenario, this would involve a call to an LLM API
        // and parsing its output to get the specific keys needed from the knowledge base.

        // For now, directly call the KillTeamAnalysisService
        var request = new KillTeamAnalysisRequest
        {
            Team1Name = team1Name,
            Team2Name = team2Name,
            Language = language
        };

        var analysisResult = await _killTeamAnalysisService.AnalyzeTeamsAsync(request);

        // Placeholder for LLM interaction to redraft/summarize the results
        _logger.LogInformation("LLM: Redrafting analysis results...");
        // In a real scenario, this would involve another call to an LLM API
        // with the analysisResult and instructions on how to redraft it.

        return analysisResult;
    }
}
