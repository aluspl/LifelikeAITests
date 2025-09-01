using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AiAgent.Api.Domain.Knowledge.Commands;
using AiAgent.Api.Domain.Knowledge.CommandHandlers;
using AiAgent.Api.Domain.Knowledge.Queries;
using AiAgent.Api.Domain.Knowledge.QueryHandlers;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Knowledge.Models;
using AiAgent.Api.Domain.Knowledge.Enums;

namespace AiAgent.Api.Tests.Domain.Knowledge;

public class KnowledgeHandlersTests
{
    [Fact]
    public async Task CreateKnowledgeCommandHandler_ShouldInsertKnowledge()
    {
        // Arrange
        var mockKnowledgeRepository = new Mock<IKnowledgeRepository>();
        var handler = new CreateKnowledgeCommandHandler(mockKnowledgeRepository.Object);
        var request = new CreateKnowledgeRequest
        {
            Key = "TestKey",
            Module = "TestModule",
            SourceType = KnowledgeSourceType.Inline,
            Items = new List<KnowledgeItem> { new KnowledgeItem { Key = "item1", Value = "value1" } }
        };
        var command = new CreateKnowledgeCommand { Request = request };

        // Act
        await handler.HandleAsync(command);

        // Assert
        mockKnowledgeRepository.Verify(repo => repo.InsertAsync(It.Is<KnowledgeEntity>(k => 
            k.Key == request.Key && 
            k.Module == request.Module &&
            k.SourceType == request.SourceType &&
            k.Items.Count == request.Items.Count
        )), Times.Once);
    }

    [Fact]
    public async Task GetKnowledgeByIdQueryHandler_ShouldReturnKnowledge_WhenExists()
    {
        // Arrange
        var mockKnowledgeRepository = new Mock<IKnowledgeRepository>();
        var knowledgeId = Guid.NewGuid();
        var expectedKnowledge = new KnowledgeEntity { Id = knowledgeId, Key = "TestKey", Module = "TestModule" };
        mockKnowledgeRepository.Setup(repo => repo.GetByIdAsync(knowledgeId)).ReturnsAsync(expectedKnowledge);

        var handler = new GetKnowledgeByIdQueryHandler(mockKnowledgeRepository.Object);
        var query = new GetKnowledgeByIdQuery { Id = knowledgeId };

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedKnowledge.Id, result.Id);
        mockKnowledgeRepository.Verify(repo => repo.GetByIdAsync(knowledgeId), Times.Once);
    }

    [Fact]
    public async Task GetKnowledgeByIdQueryHandler_ShouldReturnNull_WhenNotExists()
    {
        // Arrange
        var mockKnowledgeRepository = new Mock<IKnowledgeRepository>();
        var knowledgeId = Guid.NewGuid();
        mockKnowledgeRepository.Setup(repo => repo.GetByIdAsync(knowledgeId)).ReturnsAsync((KnowledgeEntity)null);

        var handler = new GetKnowledgeByIdQueryHandler(mockKnowledgeRepository.Object);
        var query = new GetKnowledgeByIdQuery { Id = knowledgeId };

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.Null(result);
        mockKnowledgeRepository.Verify(repo => repo.GetByIdAsync(knowledgeId), Times.Once);
    }
}
