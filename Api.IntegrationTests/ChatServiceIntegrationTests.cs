using Api.Domain.AI.Interfaces;
using Api.Domain.Chat.Enums;
using Api.Domain.Chat.Interfaces;
using Api.Domain.Chat.Models;
using Api.Domain.Database.Entites;
using Api.Domain.Database.Interfaces;
using Api.Domain.Instructions.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MongoDB.Driver;
using Xunit;

namespace Api.IntegrationTests;

public class ChatServiceIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public ChatServiceIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ProcessAsync_ShouldReturnResponse_WithMockedAiService()
    {
        // Arrange
        var client = _factory.CreateClient();

        var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
        using var scope = scopeFactory.CreateScope();
        var chatService = scope.ServiceProvider.GetRequiredService<IChatService>();

        var request = new ChatRequest
        {
            UserMessage = "Hello from integration test",
            Provider = AiProvider.Gemini,
            Instructions = "Test instructions"
        };

        // Act
        var result = await chatService.ProcessAsync(request);

        // Assert
        Assert.Equal("Mocked AI Response", result);
        // Verification of IAiService.ProcessAsync is done within CustomWebApplicationFactory
        // Verification of IChatHistoryRepository.InsertAsync is done within CustomWebApplicationFactory
    }
}