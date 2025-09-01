using AiAgent.Api.Domain.AI.Interfaces;
using AiAgent.Api.Domain.Chat.Enums;

namespace Api.IntegrationTests.MockServices;

public class MockAiProvider(AiProvider provider) : IAiProvider
{
    public AiProvider ProviderType { get; set; } = provider;

    public Task<string> GetChatCompletionAsync(string userMessage, string instructions)
    {
        return Task.FromResult("{\"content\":\"AI processed result\"}");
    }
}