using AiAgent.Api.Domain.AI.Interfaces;
using AiAgent.Api.Domain.Chat.Enums;
using AiAgent.Api.Domain.Chat.Models;
using AiAgent.Api.Domain.Chat.Services;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Instructions.Interfaces;
using Moq;
using Xunit;

namespace AiAgent.Api.Tests;

public class ChatServiceTests
{
    private readonly Mock<IChatHistoryRepository> _mockChatHistoryRepository;
    private readonly Mock<IInstructionService> _mockInstructionService;
    private readonly ChatService _chatService;
    private readonly Mock<IAiService> _mockAiService;

    public ChatServiceTests()
    {
        _mockChatHistoryRepository = new Mock<IChatHistoryRepository>();
        _mockInstructionService = new Mock<IInstructionService>();
        _mockAiService = new Mock<IAiService>();

        Func<string, IAiService> aiServiceFactory = (provider) =>
        {
            if (provider == AiProvider.Gemini.ToString().ToLower() || provider == AiProvider.OpenAi.ToString().ToLower())
            {
                return _mockAiService.Object;
            }
            return null;
        };

        _chatService = new ChatService(
            aiServiceFactory,
            _mockChatHistoryRepository.Object,
            _mockInstructionService.Object
        );
    }

    [Fact]
    public async Task ProcessAsync_ShouldReturnResponse_WhenInstructionsProvided()
    {
        // Arrange
        var request = new ChatRequest
        {
            UserMessage = "Hello",
            Provider = AiProvider.Gemini,
            Instructions = "Some instructions"
        };
        var expectedResponse = "AI response";

        _mockAiService.Setup(s => s.ProcessAsync(It.IsAny<string>(), It.IsAny<string>()))
                     .ReturnsAsync(expectedResponse);

        // Act
        var result = await _chatService.ProcessAsync(request);

        // Assert
        Assert.Equal(expectedResponse, result);
        _mockAiService.Verify(s => s.ProcessAsync(request.UserMessage, request.Instructions), Times.Once);
        _mockChatHistoryRepository.Verify(r => r.InsertAsync(It.IsAny<AiAgent.Api.Domain.Database.Entites.ChatHistoryEntity>()), Times.Once);
        _mockInstructionService.Verify(s => s.GetInstructionAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task ProcessAsync_ShouldFetchInstructions_WhenInstructionsNotProvided()
    {
        // Arrange
        var request = new ChatRequest
        {
            UserMessage = "Hello",
            Provider = AiProvider.OpenAi,
            Instructions = null
        };
        var fetchedInstructions = "Fetched instructions";
        var expectedResponse = "AI response";

        _mockAiService.Setup(s => s.ProcessAsync(It.IsAny<string>(), It.IsAny<string>()))
                     .ReturnsAsync(expectedResponse);

        _mockInstructionService.Setup(s => s.GetInstructionAsync(It.IsAny<string>()))
                               .ReturnsAsync(fetchedInstructions);

        // Act
        var result = await _chatService.ProcessAsync(request);

        // Assert
        Assert.Equal(expectedResponse, result);
        _mockAiService.Verify(s => s.ProcessAsync(request.UserMessage, fetchedInstructions), Times.Once);
        _mockChatHistoryRepository.Verify(r => r.InsertAsync(It.IsAny<AiAgent.Api.Domain.Database.Entites.ChatHistoryEntity>()), Times.Once);
        _mockInstructionService.Verify(s => s.GetInstructionAsync(request.Provider.ToString().ToLower()), Times.Once);
    }

    [Fact]
    public async Task ProcessAsync_ShouldThrowException_WhenAiServiceNotFound()
    {
        // Arrange
        var request = new ChatRequest
        {
            UserMessage = "Hello",
            Provider = AiProvider.Gemini,
            Instructions = "Some instructions"
        };

        // To simulate AI service not found, we'll make the factory return null for this specific provider
        Func<string, IAiService> aiServiceFactoryForNotFound = (provider) => null;

        var chatServiceWithNotFound = new ChatService(
            aiServiceFactoryForNotFound,
            _mockChatHistoryRepository.Object,
            _mockInstructionService.Object
        );

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => chatServiceWithNotFound.ProcessAsync(request));
        Assert.Equal($"Provider '{request.Provider}' not found.", exception.Message);
        _mockChatHistoryRepository.Verify(r => r.InsertAsync(It.IsAny<AiAgent.Api.Domain.Database.Entites.ChatHistoryEntity>()), Times.Never);
    }

    [Fact]
    public async Task ProcessAsync_ShouldSaveChatHistoryCorrectly()
    {
        // Arrange
        var request = new ChatRequest
        {
            UserMessage = "Test message",
            Provider = AiProvider.Gemini,
            Instructions = "Test instructions"
        };
        var expectedResponse = "Test AI response";

        _mockAiService.Setup(s => s.ProcessAsync(It.IsAny<string>(), It.IsAny<string>()))
                     .ReturnsAsync(expectedResponse);

        // Act
        await _chatService.ProcessAsync(request);

        // Assert
        _mockChatHistoryRepository.Verify(r => r.InsertAsync(It.Is<Api.Domain.Database.Entites.ChatHistoryEntity>(
            ch => ch.Query == request.UserMessage &&
                  ch.Response == expectedResponse &&
                  ch.Model == request.Provider.ToString() &&
                  ch.Role == "assistant"
        )), Times.Once);
    }
}