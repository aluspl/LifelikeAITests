using AiAgent.Api;
using AiAgent.Api.Domain.AI.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using AiAgent.Api.Domain.Chat.Enums;
using Api.IntegrationTests.MockServices;
using Microsoft.Extensions.Configuration;
using Testcontainers.MongoDb;

namespace Api.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private MongoDbContainer _mongoDbContainer;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, conf) =>
        {
            conf.AddInMemoryCollection(new Dictionary<string, string>
            {
                {"DefaultApiKey:Key", "test-api-key"}
            });
            conf.AddInMemoryCollection(new Dictionary<string, string>
            {
                {"MongoDB:ConnectionString", _mongoDbContainer.GetConnectionString()},
                {"MongoDB:DatabaseName", "test_db"}
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
            services.AddKeyedTransient<IAiProvider>(nameof(AiProvider.OpenAi).ToLower(), (sp, key) => new MockAiProvider(AiProvider.OpenAi));
            services.AddKeyedTransient<IAiProvider>(nameof(AiProvider.Gemini).ToLower(), (sp, key) => new MockAiProvider(AiProvider.Gemini));
        });
    }

    public async Task InitializeAsync()
    {
        _mongoDbContainer = new MongoDbBuilder()
            .WithImage("mongo:latest")
            .WithExposedPort(27017) // Expose default MongoDB port
            .Build();

        await _mongoDbContainer.StartAsync();
    }
    
    public new async Task DisposeAsync()
    {
        await ClearDatabaseAsync(); // Clear database before disposing
        if (_mongoDbContainer != null)
        {
            await _mongoDbContainer.StopAsync();
            await _mongoDbContainer.DisposeAsync();
        }
    }
    
    public async Task ClearDatabaseAsync()
    {
        if (_mongoDbContainer != null)
        {
            var connectionString = _mongoDbContainer.GetConnectionString();
            var client = new MongoDB.Driver.MongoClient(connectionString);
            await client.DropDatabaseAsync("test_db"); // Use the same database name as in config
        }
    }
    
    public HttpClient CreateClientWithApiKey(string apiKey)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
        return client;
    }
}