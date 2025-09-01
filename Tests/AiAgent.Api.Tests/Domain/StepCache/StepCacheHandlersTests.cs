using Xunit;
using Moq;
using System.Threading.Tasks;
using System.Collections.Generic;
using AiAgent.Api.Domain.StepCache.Queries;
using AiAgent.Api.Domain.StepCache.QueryHandlers;
using AiAgent.Api.Domain.StepCache.Interfaces;
using AiAgent.Api.Domain.StepCache.Models;

namespace AiAgent.Api.Tests.Domain.StepCache;

public class StepCacheHandlersTests
{
    [Fact]
    public async Task GetAllStepCachesQueryHandler_ShouldReturnAllStepCaches()
    {
        // Arrange
        var mockStepCacheService = new Mock<IStepCacheService>();
        var expectedCaches = new List<StepCacheResponse>
        {
            new StepCacheResponse { Id = Guid.NewGuid(), AgentStepId = Guid.NewGuid(), Query = "q1", Value = "v1" },
            new StepCacheResponse { Id = Guid.NewGuid(), AgentStepId = Guid.NewGuid(), Query = "q2", Value = "v2" }
        };
        mockStepCacheService.Setup(s => s.GetAllStepCaches()).ReturnsAsync(expectedCaches);

        var handler = new GetAllStepCachesQueryHandler(mockStepCacheService.Object);
        var query = new GetAllStepCachesQuery();

        // Act
        var result = await handler.HandleAsync(query);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedCaches.Count, result.Count);
        Assert.Contains(expectedCaches[0], result);
        Assert.Contains(expectedCaches[1], result);
        mockStepCacheService.Verify(s => s.GetAllStepCaches(), Times.Once);
    }
}
