using AiAgent.Api;
using AiAgent.Api.Domain.AI.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using AiAgent.Api.Domain.Chat.Enums;
using Api.IntegrationTests.MockServices;
using Microsoft.Extensions.Configuration;

namespace Api.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<IAiProvider> AiServiceGeminiMock { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, conf) =>
        {
            conf.AddInMemoryCollection(new Dictionary<string, string>
            {
                {"DefaultApiKey:Key", "test-api-key"}
            });
        });

        builder.ConfigureServices(services =>
        {
            var aiServiceDescriptors = services.Where(d => 
                d.ServiceType == typeof(IAiProvider) && d.IsKeyedService
            ).ToList();

            foreach (var descriptor in aiServiceDescriptors)
            {
                services.Remove(descriptor);
            }

            // Add the mock IAiProvider as keyed services
            services.AddKeyedTransient<IAiProvider, MockAiProvider>(nameof(AiProvider.OpenAi).ToLower());
            services.AddKeyedTransient<IAiProvider, MockAiProvider>(nameof(AiProvider.Gemini).ToLower());
        });
    }

    public HttpClient CreateClientWithApiKey(string apiKey)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
        return client;
    }
}