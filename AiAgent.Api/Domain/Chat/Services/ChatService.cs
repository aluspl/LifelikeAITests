using AiAgent.Api.Domain.AI.Interfaces;
using AiAgent.Api.Domain.Chat.Enums;
using AiAgent.Api.Domain.Chat.Interfaces;
using AiAgent.Api.Domain.Chat.Models;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Instructions.Interfaces;

namespace AiAgent.Api.Domain.Chat.Services;

public class ChatService(Func<string, IAiService> aiServiceFactory, IChatHistoryRepository chatHistoryRepository, IInstructionService instructionService) : IChatService
{
    public async Task<string> ProcessAsync(ChatRequest request)
    {
        var provider = request.Provider.ToString().ToLower();
        var aiService = aiServiceFactory(provider);
        if (aiService is null)
        {
            throw new Exception($"Provider '{request.Provider}' not found.");
        }

        var instructions = request.Instructions;
        if (string.IsNullOrEmpty(instructions))
        {
            instructions = await instructionService.GetInstructionAsync(provider);
        }

        var response = await aiService.ProcessAsync(request.UserMessage, instructions);

        await SaveChatHistory(request.UserMessage, response, request.Provider);

        return response;
    }

    private async Task SaveChatHistory(string userMessage, string assistantResponse, AiProvider provider)
    {
        var chatHistory = new ChatHistoryEntity
        {
            Query = userMessage,
            Response = assistantResponse,
            Model = provider.ToString(),
            Created = DateTime.UtcNow,
            Role = "assistant"
        };
        await chatHistoryRepository.InsertAsync(chatHistory);
    }
}
