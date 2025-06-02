using Api.Domain.AI.Interfaces;
using Api.Domain.Configuration;
using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using OpenAI.Chat;

namespace Api.Domain.AI.Services;

public class OpenAiService : IOpenAiService
{
    private readonly OpenAiSettings _settings;
    private AzureOpenAIClient _azureClient;
    private ChatClient _chatClient;
    private List<ChatMessage> _chatMessages = new();

    public OpenAiService(IOptions<OpenAiSettings> settings)
    {
        _settings = settings.Value;

        _azureClient = new AzureOpenAIClient(new Uri(_settings.Endpoint), new AzureKeyCredential(_settings.ApiKey));
        _chatClient = _azureClient.GetChatClient(_settings.Deployment);
    }

    public async Task<string> Process(string userMessage)
    {
        // Add the user message to the chat history
        _chatMessages.Add(new UserChatMessage(userMessage));

        // Get chat completion
        ChatCompletion completion = await _chatClient.CompleteChatAsync(_chatMessages);

        // Add the assistant's response to the chat history
        _chatMessages.Add(new AssistantChatMessage(completion.Content[0].Text));

        // Print the response
        Console.WriteLine($"{completion.Role}: {completion.Content[0].Text}");

        return $"{completion.Role}: {completion.Content[0].Text}";
    }
}