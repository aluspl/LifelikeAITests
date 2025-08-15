using AiAgent.Api.Domain.AI.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Instructions.Interfaces;
using AiAgent.Api.Domain.Knowledge.Enums;
using AiAgent.Api.Domain.Knowledge.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AiAgent.Api.Domain.Agents.Orchestration
{
    public class AgentOrchestratorService : IAgentOrchestratorService
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IAgentStepRepository _agentStepRepository;
        private readonly IInstructionRepository _instructionRepository;
        private readonly IKnowledgeRepository _knowledgeRepository;
        private readonly IEnumerable<IAiService> _aiServices;
        private readonly IExecutionLogRepository _executionLogRepository;

        public AgentOrchestratorService(
            IAgentRepository agentRepository,
            IAgentStepRepository agentStepRepository,
            IInstructionRepository instructionRepository,
            IKnowledgeRepository knowledgeRepository,
            IEnumerable<IAiService> aiServices,
            IExecutionLogRepository executionLogRepository)
        {
            _agentRepository = agentRepository;
            _agentStepRepository = agentStepRepository;
            _instructionRepository = instructionRepository;
            _knowledgeRepository = knowledgeRepository;
            _aiServices = aiServices;
            _executionLogRepository = executionLogRepository;
        }

        public async Task<string> ExecuteAgent(string agentId, string initialInput)
        {
            var agent = await _agentRepository.GetByIdAsync(agentId);
            if (agent == null)
            {
                throw new ArgumentException($"Agent with ID {agentId} not found.");
            }

            var steps = new List<AgentStepEntity>();
            foreach (var stepId in agent.StepIds)
            {
                var step = await _agentStepRepository.GetByIdAsync(stepId);
                if (step == null)
                {
                    throw new ArgumentException($"AgentStep with ID {stepId} not found for agent {agent.Name}.");
                }
                steps.Add(step);
            }

            var correlationId = Guid.NewGuid().ToString();
            var executionLog = new ExecutionLogEntity
            {
                AgentId = agentId,
                CorrelationId = correlationId,
                StartTime = DateTime.UtcNow,
                StepResults = new List<StepExecutionResult>()
            };

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
                    var cachedResult = await _executionLogRepository.GetOneAsync(
                        log => log.AgentId == agentId &&
                               log.CorrelationId != correlationId &&
                               log.StepResults.Any(sr => sr.StepName == step.Name && sr.Input == currentInput && sr.WasCached));

                    if (cachedResult != null && cachedResult.StepResults.Any())
                    {
                        var previousStepResult = cachedResult.StepResults.First(sr => sr.StepName == step.Name && sr.Input == currentInput);
                        stepResult.Output = previousStepResult.Output;
                        stepResult.Content = previousStepResult.Content;
                        stepResult.WasCached = true;
                        currentInput = stepResult.Content;
                    }
                }

                if (!stepResult.WasCached)
                {
                    var instruction = await _instructionRepository.GetByIdAsync(step.InstructionId);
                    if (instruction == null)
                    {
                        throw new ArgumentException($"Instruction with ID {step.InstructionId} not found for step {step.Name}.");
                    }

                    var knowledgeContentBuilder = new StringBuilder();
                    foreach (var knowledgeId in step.KnowledgeIds)
                    {
                        var knowledge = await _knowledgeRepository.GetByIdAsync(knowledgeId);
                        if (knowledge != null)
                        {
                            if (knowledge.SourceType == KnowledgeSourceType.Inline)
                            {
                                foreach (var item in knowledge.Items)
                                {
                                    knowledgeContentBuilder.AppendLine($"{item.Key}: {item.Value}");
                                }
                            }
                            else if (knowledge.SourceType == KnowledgeSourceType.BlobUrl)
                            {
                                // TODO: Implement fetching content from BlobUrl
                                knowledgeContentBuilder.AppendLine($"Knowledge from Blob URL: {knowledge.BlobUrl} (fetching not implemented)");
                            }
                        }
                    }

                    var aiService = _aiServices.FirstOrDefault(s => s.Provider.ToString().Equals(step.ModelProvider.ToString(), StringComparison.OrdinalIgnoreCase));
                    if (aiService == null)
                    {
                        throw new InvalidOperationException($"AI Service for provider {step.ModelProvider} not found.");
                    }

                    var promptBuilder = new StringBuilder();
                    promptBuilder.AppendLine(instruction.Content);
                    promptBuilder.AppendLine();
                    promptBuilder.AppendLine("Knowledge:");
                    promptBuilder.AppendLine(knowledgeContentBuilder.ToString());
                    promptBuilder.AppendLine();
                    promptBuilder.AppendLine("Input:");
                    promptBuilder.AppendLine(currentInput);
                    var prompt = promptBuilder.ToString();

                    var aiResponse = await aiService.GetChatCompletionAsync(prompt, instruction.Content);
                    stepResult.Output = aiResponse;

                    try
                    {
                        using (JsonDocument doc = JsonDocument.Parse(aiResponse))
                        {
                            if (doc.RootElement.TryGetProperty("Content", out JsonElement contentElement))
                            {
                                stepResult.Content = contentElement.GetString();
                            }
                            else
                            {
                                stepResult.Content = aiResponse; // Fallback if 'Content' field not found
                            }
                        }
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
            await _executionLogRepository.InsertAsync(executionLog);

            return finalOutput;
        }
    }
}