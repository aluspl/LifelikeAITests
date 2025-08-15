using Xunit;
using Moq;
using AiAgent.Api.Domain.Knowledge.Services;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Knowledge.Enums;
using AiAgent.Api.Domain.Knowledge.Models;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace AiAgent.Api.Tests
{
    public class DataKnowledgeServiceTests
    {
        private readonly Mock<IKnowledgeRepository> _mockKnowledgeRepository;
        private readonly DataKnowledgeService _service;

        public DataKnowledgeServiceTests()
        {
            _mockKnowledgeRepository = new Mock<IKnowledgeRepository>();
            _service = new DataKnowledgeService(_mockKnowledgeRepository.Object);
        }

        [Fact]
        public async Task UploadKnowledgeDataAsync_ShouldProcessJsonlAndUpsertKnowledge()
        {
            // Arrange
            var jsonlContent = "{\"contents\":[{\"role\":\"user\",\"parts\":[{\"text\":\"key1\"}]},{\"role\":\"model\",\"parts\":[{\"text\":\"value1\"}]}]}";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonlContent));
            var module = "test-module";

            _mockKnowledgeRepository.Setup(r => r.UpsertAsync(It.IsAny<KnowledgeEntity>())).Returns(Task.CompletedTask);

            // Act
            var count = await _service.UploadKnowledgeDataAsync(stream, module);

            // Assert
            Assert.Equal(1, count);
            _mockKnowledgeRepository.Verify(r => r.UpsertAsync(It.Is<KnowledgeEntity>(kb => 
                kb.Module == module && 
                kb.SourceType == KnowledgeSourceType.Inline &&
                kb.Items.Any(item => item.Key == "key1" && item.Value == "value1")
            )), Times.Once);
        }

        [Fact]
        public async Task UploadKnowledgeDataAsync_ShouldReturnZeroForEmptyStream()
        {
            // Arrange
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(""));
            var module = "test-module";

            // Act
            var count = await _service.UploadKnowledgeDataAsync(stream, module);

            // Assert
            Assert.Equal(0, count);
            _mockKnowledgeRepository.Verify(r => r.UpsertAsync(It.IsAny<KnowledgeEntity>()), Times.Never);
        }

        [Fact]
        public async Task UploadBlobKnowledgeAsync_ShouldUpsertKnowledgeWithBlobUrl()
        {
            // Arrange
            var key = "blob-key";
            var module = "blob-module";
            var blobUrl = "http://example.com/blob";

            _mockKnowledgeRepository.Setup(r => r.UpsertAsync(It.IsAny<KnowledgeEntity>())).Returns(Task.CompletedTask);

            // Act
            await _service.UploadBlobKnowledgeAsync(key, module, blobUrl);

            // Assert
            _mockKnowledgeRepository.Verify(r => r.UpsertAsync(It.Is<KnowledgeEntity>(kb => 
                kb.Key == key && 
                kb.Module == module && 
                kb.SourceType == KnowledgeSourceType.BlobUrl && 
                kb.BlobUrl == blobUrl
            )), Times.Once);
        }
    }
}