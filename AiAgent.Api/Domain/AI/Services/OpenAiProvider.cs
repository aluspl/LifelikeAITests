using AiAgent.Api.Domain.AI.Interfaces;
using AiAgent.Api.Domain.Configuration;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using OpenAI.Chat;
using AiAgent.Api.Domain.Chat.Enums;

namespace AiAgent.Api.Domain.AI.Services;

public class OpenAiProvider : IAiProvider
{
    private readonly ChatClient _chatClient;

    public static AiProvider Provider => AiProvider.OpenAi;
    public AiProvider ProviderType => Provider;

    public OpenAiProvider(IOptions<OpenAiSettings> settings)
    {
        var settings1 = settings.Value;

        var azureClient = new AzureOpenAIClient(new Uri(settings1.Endpoint), new AzureKeyCredential(settings1.ApiKey));
        _chatClient = azureClient.GetChatClient(settings1.Deployment);
    }

    public async Task<string> GetChatCompletionAsync(string userMessage, string instructions)
    {
        var chatMessages = new List<ChatMessage>();

        // Add the instructions message to the chat history
        chatMessages.Add(new SystemChatMessage(instructions));
        
        // Add the user message to the chat history
        chatMessages.Add(new UserChatMessage(userMessage));

        // Get chat completion
        ChatCompletion completion = await _chatClient.CompleteChatAsync(chatMessages);

        // Print the response
        Console.WriteLine($"{completion.Role}: {completion.Content[0].Text}");

        return $"{completion.Role}: {completion.Content[0].Text}";
    }
}