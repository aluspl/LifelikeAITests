using System.Text.Json;
using Api.Domain.AI.Interfaces;
using Api.Domain.Configuration;
using Api.Domain.Database.Entites;
using Api.Domain.Database.Interfaces;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;

namespace Api.Domain.AI.Services;

public class GeminiService(IOptions<GeminiSettings> settings, IChatHistoryRepository chatHistoryRepository) : IGeminiService
{
    private readonly GeminiSettings _settings = settings.Value;

    public async Task<string> Process(string userMessage)
    {
        try
        {
            // Create user message
            var userContent = new
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

            // Make API call using Flurl
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_settings.Model}:generateContent";
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

            // Save chat history to MongoDB
            var chatHistory = new ChatHistoryEntity
            {
                Query = userMessage,
                Response = assistantResponse,
                Model = _settings.Model,
                Created = DateTime.UtcNow,
                Role = "assistant"
            };
            await chatHistoryRepository.InsertAsync(chatHistory);

            return $"assistant: {assistantResponse}";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Gemini service: {ex.Message}");
            return $"Error: {ex.Message}";
        }
    }
}