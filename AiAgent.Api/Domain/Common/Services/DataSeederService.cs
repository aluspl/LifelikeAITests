using AiAgent.Api.Domain.Common.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using Microsoft.Extensions.Options;
using AiAgent.Api.Domain.Configuration;
using System.IO;
using System.Text.Json;
using AiAgent.Api.Domain.Chat.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using AiAgent.Api.Domain.Knowledge.Enums;
using AiAgent.Api.Domain.Knowledge.Models;
using System.Linq;

namespace AiAgent.Api.Domain.Common.Services
{
    public class DataSeederService : IDataSeederService
    {
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly ApiKeySettings _apiKeySettings;
        private readonly IInstructionRepository _instructionRepository;
        private readonly IKnowledgeRepository _knowledgeRepository;
        private readonly IAgentRepository _agentRepository;
        private readonly IAgentStepRepository _agentStepRepository;

        public DataSeederService(IKnowledgeRepository knowledgeRepository, 
                                 IApiKeyRepository apiKeyRepository, 
                                 IOptions<ApiKeySettings> apiKeyOptions, 
                                 IInstructionRepository instructionRepository, 
                                 IAgentRepository agentRepository,
                                 IAgentStepRepository agentStepRepository)
        {
            _knowledgeRepository = knowledgeRepository;
            _apiKeyRepository = apiKeyRepository;
            _apiKeySettings = apiKeyOptions.Value;
            _instructionRepository = instructionRepository;
            _agentRepository = agentRepository;
            _agentStepRepository = agentStepRepository;
        }

        public async Task SeedAllDataAsync()
        {
            await SeedApiKeysAsync();
            await SeedKillTeamAnalyzerAgentAsync();
        }

        public async Task SeedApiKeysAsync()
        {
            var existingKey = await _apiKeyRepository.GetByKeyAsync(_apiKeySettings.Key); // Use GetByKeyAsync
            if (existingKey == null)
            {
                var apiKey = new ApiKey
                {
                    Key = _apiKeySettings.Key, // Use Key
                    Owner = _apiKeySettings.Owner,
                    Created = DateTime.UtcNow,
                    Expires = null // Never expires
                };
                await _apiKeyRepository.InsertAsync(apiKey);
            }
        }

        public async Task SeedKillTeamAnalyzerAgentAsync()
        {
            var existingAgent = await _agentRepository.GetOneAsync(a => a.Name == "KillTeam Analyzer");
            if (existingAgent != null)
            {
                Console.WriteLine("KillTeam Analyzer Agent already seeded. Skipping.");
                return;
            }

            var instructionEntity = await SeedInstructionAsync();

            var knowledgeBaseId = await SeedKnowledgeAsync();

            var stepIds = await SeedAgentStepsAsync(instructionEntity.Id, new List<string> { knowledgeBaseId });

            var killTeamAnalyzerAgent = new AgentEntity
            {
                Name = "KillTeam Analyzer",
                Description = "Analyses two Kill Teams and provides a tactical overview.",
                StepIds = stepIds
            };
            await _agentRepository.InsertAsync(killTeamAnalyzerAgent);

            Console.WriteLine($"Successfully seeded KillTeam Analyzer Agent with ID: {killTeamAnalyzerAgent.Id}");
        }

        private async Task<InstructionEntity> SeedInstructionAsync()
        {
            var instructionFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Domain", "Instructions", "HighFidelityAgentInstructions.md");
            if (!File.Exists(instructionFilePath)) { instructionFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Domain", "Instructions", "HighFidelityAgentInstructions.md"); }
            if (!File.Exists(instructionFilePath)) { instructionFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "AiAgent.Api", "Domain", "Instructions", "HighFidelityAgentInstructions.md"); }

            var instructionContent = await File.ReadAllTextAsync(instructionFilePath);
            var instructionEntity = new InstructionEntity { Key = "KillTeamAnalyzerInstruction", Content = instructionContent }; // Use Key
            await _instructionRepository.InsertAsync(instructionEntity);
            Console.WriteLine($"Seeded Instruction: {instructionEntity.Key} with ID: {instructionEntity.Id}"); // Use Key
            return instructionEntity;
        }

        private async Task<string> SeedKnowledgeAsync()
        {
            var knowledgeJsonlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "trained_killteamjson_data.jsonl");
            if (!File.Exists(knowledgeJsonlFilePath)) { knowledgeJsonlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Assets", "trained_killteamjson_data.jsonl"); }
            if (!File.Exists(knowledgeJsonlFilePath)) { knowledgeJsonlFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Assets", "trained_killteamjson_data.jsonl"); }

            var knowledgeItems = new List<KnowledgeItem>();
            var jsonlLines = await File.ReadAllLinesAsync(knowledgeJsonlFilePath);
            foreach (var line in jsonlLines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                using (JsonDocument doc = JsonDocument.Parse(line))
                {
                    if (doc.RootElement.TryGetProperty("Key", out var keyElement) && doc.RootElement.TryGetProperty("Content", out var contentElement))
                    {
                        knowledgeItems.Add(new KnowledgeItem
                        {
                            Key = keyElement.GetString(),
                            Value = contentElement.GetString()
                        });
                    }
                }
            }

            var knowledgeEntity = new KnowledgeEntity
            {
                Key = "KillTeamKnowledge", // Use Key
                Module = "KillTeam",
                SourceType = KnowledgeSourceType.Inline,
                Items = knowledgeItems,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };
            await _knowledgeRepository.UpsertAsync(knowledgeEntity);
            Console.WriteLine($"Seeded KnowledgeBase: {knowledgeEntity.Key} with ID: {knowledgeEntity.Id}"); // Use Key
            return knowledgeEntity.Id;
        }

        private async Task<List<string>> SeedAgentStepsAsync(string instructionId, List<string> knowledgeIds)
        {
            var stepsToCreate = new List<AgentStepEntity>
            {
                new AgentStepEntity { Order = 1, Name = "Analyze Team 1", InstructionId = instructionId, KnowledgeIds = knowledgeIds, ModelProvider = AiProvider.Gemini, IsCached = true },
                new AgentStepEntity { Order = 2, Name = "Analyze Team 2", InstructionId = instructionId, KnowledgeIds = knowledgeIds, ModelProvider = AiProvider.Gemini, IsCached = true },
                new AgentStepEntity { Order = 3, Name = "Perform Tactical Analysis", InstructionId = instructionId, KnowledgeIds = knowledgeIds, ModelProvider = AiProvider.Gemini, IsCached = false }
            };

            var stepIds = new List<string>();
            foreach (var step in stepsToCreate)
            {
                await _agentStepRepository.InsertAsync(step);
                stepIds.Add(step.Id);
            }
            Console.WriteLine($"Seeded {stepIds.Count} Agent Steps.");
            return stepIds;
        }
    }
}