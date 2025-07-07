using System.Text.Json;
using AiAgent.Api.Domain.AI.Interfaces;
using AiAgent.Api.Domain.Configuration;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Options;
using Google.Apis.Auth.OAuth2;

namespace AiAgent.Api.Domain.AI.Services;

public class GeminiService(IOptions<GeminiSettings> settings) : IAiService
{
    private readonly GeminiSettings _settings = settings.Value;

    public async Task<string> ProcessAsync(string userMessage, string instructions)
    {
        try
        {
            // Create user message
            var userContent = CreateRequest(userMessage);

            // Make API call using Flurl
            var url = $"https://{_settings.Location}-aiplatform.googleapis.com/v1/projects/{_settings.ProjectId}/locations/{_settings.Location}/publishers/{_settings.Publisher}/models/{_settings.ModelId}:streamGenerateContent";
            var response = await url
                .WithOAuthBearerToken(await GetAccessToken())
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

    private static global::System.Object CreateRequest(string userMessage)
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

    private async Task<string> GetAccessToken()
    {
        var credentials = Google.Apis.Auth.OAuth2.GoogleCredential.GetApplicationDefault();
        var token = await credentials.UnderlyingCredential.GetAccessTokenForRequestAsync();
        return token;
    }
}