using System.Text;
using System.Text.Json;
using AiAgent.Api.Domain.Agents.Orchestration.Interfaces;
using AiAgent.Api.Domain.AI.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Knowledge.Enums;

namespace AiAgent.Api.Domain.Agents.Orchestration.Services
{
    public class AgentOrchestratorService(
        IAgentRepository agentRepository,
        IAgentStepRepository agentStepRepository,
        IKnowledgeRepository knowledgeRepository,
        IEnumerable<IAiProvider> aiServices,
        IExecutionLogRepository executionLogRepository)
        : IAgentOrchestratorService
    {
        public async Task<string> ExecuteAgent(Guid agentId, string initialInput)
        {
            var agent = await GetAgent(agentId);
            var steps = await GetAgentSAteps(agent);
            var executionLog = CreateExecutionLog(agentId);

            string currentInput = initialInput;
            string finalOutput = string.Empty;

            foreach (var step in steps.OrderBy(s => s.Order))
            {
                var stepStartTime = DateTime.UtcNow;
                var stepResult = new StepExecutionResult
                {
                    StepName = step.Name,
                    Input = currentInput,
                    WasCached = false
                };

                if (step.IsCached)
                {
                    var cachedResult = await executionLogRepository.GetOneAsync(log => log.AgentId == agentId &&
                        log.CorrelationId != executionLog.CorrelationId &&
                        log.StepResults.Any(sr =>
                            sr.StepName == step.Name && sr.Input == currentInput && sr.WasCached));

                    if (cachedResult != null && cachedResult.StepResults.Any())
                    {
                        var previousStepResult =
                            cachedResult.StepResults.First(sr => sr.StepName == step.Name && sr.Input == currentInput);
                        stepResult.Output = previousStepResult.Output;
                        stepResult.Content = previousStepResult.Content;
                        stepResult.WasCached = true;
                        currentInput = stepResult.Content;
                    }
                }

                if (!stepResult.WasCached)
                {
                    var knowledgeContentBuilder = new StringBuilder();
                    foreach (var knowledgeId in step.KnowledgeIds)
                    {
                        var knowledge = await knowledgeRepository.GetByIdAsync(knowledgeId);
                        if (knowledge == null)
                        {
                            continue;
                        }

                        switch (knowledge.SourceType)
                        {
                            case KnowledgeSourceType.Inline:
                            {
                                foreach (var item in knowledge.Items)
                                {
                                    knowledgeContentBuilder.AppendLine($"{item.Key}: {item.Value}");
                                }

                                break;
                            }
                            case KnowledgeSourceType.BlobUrl:
                                // TODO: Implement fetching content from BlobUrl
                                knowledgeContentBuilder.AppendLine(
                                    $"Knowledge from Blob URL: {knowledge.BlobUrl} (fetching not implemented)");
                                break;
                        }
                    }

                    IAiProvider aiProvider = GetAiService(step);

                    var promptBuilder = new StringBuilder();
                    // promptBuilder.AppendLine(agent.Instruction);
                    promptBuilder.AppendLine();
                    promptBuilder.AppendLine("Knowledge:");
                    promptBuilder.AppendLine(knowledgeContentBuilder.ToString());
                    promptBuilder.AppendLine();
                    promptBuilder.AppendLine("Input:");
                    promptBuilder.AppendLine(currentInput);
                    var prompt = promptBuilder.ToString();

                    var aiResponse = await aiProvider.GetChatCompletionAsync(prompt, string.Empty);
                    stepResult.Output = aiResponse;

                    try
                    {
                        using var doc = JsonDocument.Parse(aiResponse);
                        stepResult.Content = doc.RootElement.TryGetProperty("Content", out JsonElement contentElement)
                            ? contentElement.GetString()
                            : aiResponse; // Fallback if 'Content' field not found
                    }
                    catch (JsonException)
                    {
                        stepResult.Content = aiResponse; // Not a valid JSON, use raw response
                    }

                    currentInput = stepResult.Content;
                }

                stepResult.Duration = DateTime.UtcNow - stepStartTime;
                executionLog.StepResults.Add(stepResult);
                finalOutput = stepResult.Content;
            }

            executionLog.EndTime = DateTime.UtcNow;
            await executionLogRepository.InsertAsync(executionLog);

            return finalOutput;
        }

        private static ExecutionLogEntity CreateExecutionLog(Guid agentId)
        {
            var correlationId = Guid.NewGuid();
            var executionLog = new ExecutionLogEntity
            {
                AgentId = agentId,
                CorrelationId = correlationId,
                StartTime = DateTime.UtcNow,
                StepResults = []
            };
            return executionLog;
        }

        private async Task<List<AgentStepEntity>> GetAgentSAteps(AgentEntity agent)
        {
            var steps = new List<AgentStepEntity>();
            foreach (var stepId in agent.StepIds)
            {
                var step = await agentStepRepository.GetByIdAsync(stepId) ??
                           throw new ArgumentException($"AgentStep with ID {stepId} not found for agent {agent.Name}.");
                steps.Add(step);
            }

            return steps;
        }

        private async Task<AgentEntity> GetAgent(Guid agentId)
        {
            return await agentRepository.GetByIdAsync(agentId) ??
                   throw new ArgumentException($"Agent with ID {agentId} not found.");
        }

        private IAiProvider GetAiService(AgentStepEntity step)
        {
            var aiService = aiServices.FirstOrDefault(s => s.ProviderType == step.ModelProvider)
                            ?? throw new InvalidOperationException(
                                $"AI Service for provider {step.ModelProvider} not found.");
            return aiService;
        }
    }
}