using AiAgent.Api.Domain.AI.Interfaces;
using AiAgent.Api.Domain.Configuration;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace AiAgent.Api.Domain.AI.Services;

public class OpenAiService : IAiService
{
    private readonly OpenAiSettings _settings;
    private AzureOpenAIClient _azureClient;
    private ChatClient _chatClient;

    public OpenAiService(IOptions<OpenAiSettings> settings)
    {
        _settings = settings.Value;

        _azureClient = new AzureOpenAIClient(new Uri(_settings.Endpoint), new AzureKeyCredential(_settings.ApiKey));
        _chatClient = _azureClient.GetChatClient(_settings.Deployment);
    }

    public async Task<string> ProcessAsync(string userMessage, string instructions)
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