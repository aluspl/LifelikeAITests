using System.Text.Json;
using AiAgent.Api.Domain.AI.Interfaces;
using AiAgent.Api.Domain.Configuration;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Flurl;
using AiAgent.Api.Domain.Chat.Enums;

namespace AiAgent.Api.Domain.AI.Services;

public class GeminiService(IOptions<GeminiSettings> settings) : IAiService
{
    private readonly GeminiSettings _settings = settings.Value;

    public AiProvider Provider => AiProvider.Gemini;

    public async Task<string> GetChatCompletionAsync(string userMessage, string instructions)
    {
        try
        {
            // Create user message
            var userContent = CreateRequest(userMessage);

            // Make API call using Flurl
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_settings.ModelId}:generateContent";
            var response = await url
                .SetQueryParam("key", _settings.ApiKey)
                .PostJsonAsync(userContent)
                .ReceiveString();

            // Deserialize the response
            var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(response);

            // Extract the assistant's response
            var assistantResponse = geminiResponse.Candidates[0].Content.Parts[0].Text;

            if (string.IsNullOrEmpty(assistantResponse))
            {
                throw new Exception("No response received from Gemini API or response is empty");
            }

            return $"assistant: {assistantResponse}";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Gemini service: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }

    private static object CreateRequest(string userMessage)
    {
        return new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = userMessage }
                    }
                }
            }
        };
    }
}
