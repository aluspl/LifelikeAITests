using AiAgent.Api.Domain.AI.Interfaces;
using AiAgent.Api.Domain.AI.Services;
using AiAgent.Api.Domain.Chat.Enums;

namespace AiAgent.Api.Domain.AI;

public static class AiProviderRegistry
{
    public static IReadOnlyDictionary<AiProvider, Type> GetProviders()
    {
        return new Dictionary<AiProvider, Type>
        {
            { AiProvider.OpenAi, typeof(OpenAiProvider) },
            { AiProvider.Gemini, typeof(GeminiProvider) }
            // Dodaj tutaj nowych dostawców
        };
    }
}
