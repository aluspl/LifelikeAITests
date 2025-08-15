using Xunit;
using Moq;
using AiAgent.Api.Controllers;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Knowledge.Enums;
using AiAgent.Api.Domain.Knowledge.Interfaces;
using AiAgent.Api.Domain.Knowledge.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using static AiAgent.Api.Controllers.KnowledgeController; // Changed
using System;

namespace AiAgent.Api.Tests
{
    public class KnowledgeControllerTests // Changed
    {
        private readonly Mock<IKnowledgeRepository> _mockKnowledgeRepo;
        private readonly Mock<IDataKnowledgeService> _mockDataKnowledgeService;
        private readonly KnowledgeController _controller; // Changed

        public KnowledgeControllerTests()
        {
            _mockKnowledgeRepo = new Mock<IKnowledgeRepository>();
            _mockDataKnowledgeService = new Mock<IDataKnowledgeService>();
            _controller = new KnowledgeController(_mockKnowledgeRepo.Object, _mockDataKnowledgeService.Object); // Changed
        }

        [Fact]
        public async Task GetKnowledge_ShouldReturnAllKnowledge()
        {
            // Arrange
            var knowledge = new List<KnowledgeEntity> { new KnowledgeEntity(), new KnowledgeEntity() }; // Changed
            _mockKnowledgeRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(knowledge);

            // Act
            var result = await _controller.GetKnowledge(); // Changed

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            var returnedKnowledge = Assert.IsAssignableFrom<IEnumerable<KnowledgeEntity>>(okResult.Value); // Changed
            Assert.Equal(2, new List<KnowledgeEntity>(returnedKnowledge).Count);
        }

        [Fact]
        public async Task GetKnowledge_ShouldReturnKnowledge_WhenExists()
        {
            // Arrange
            var id = "test-id";
            var knowledge = new KnowledgeEntity { Id = id }; // Changed
            _mockKnowledgeRepo.Setup(repo => repo.GetByIdAsync(id)).ReturnsAsync(knowledge);

            // Act
            var result = await _controller.GetKnowledge(id); // Changed

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
            var returnedKnowledge = Assert.IsType<KnowledgeEntity>(okResult.Value); // Changed
            Assert.Equal(id, returnedKnowledge.Id);
        }

        [Fact]
        public async Task GetKnowledge_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            _mockKnowledgeRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<string>())).ReturnsAsync((KnowledgeEntity)null); // Changed

            // Act
            var result = await _controller.GetKnowledge("non-existent-id"); // Changed

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateKnowledge_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var newKnowledge = new KnowledgeEntity { Id = Guid.NewGuid().ToString() }; // Changed

            // Act
            var result = await _controller.CreateKnowledge(newKnowledge); // Changed

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(KnowledgeController.GetKnowledge), createdAtActionResult.ActionName); // Changed
            _mockKnowledgeRepo.Verify(repo => repo.InsertAsync(newKnowledge), Times.Once);
        }

        [Fact]
        public async Task UpdateKnowledge_ShouldReturnNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var id = "test-id";
            var knowledgeToUpdate = new KnowledgeEntity { Id = id }; // Changed

            // Act
            var result = await _controller.UpdateKnowledge(id, knowledgeToUpdate); // Changed

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockKnowledgeRepo.Verify(repo => repo.UpdateAsync(knowledgeToUpdate), Times.Once);
        }

        [Fact]
        public async Task DeleteKnowledge_ShouldReturnNoContent_WhenDeleteIsSuccessful()
        {
            // Arrange
            var id = "test-id";

            // Act
            var result = await _controller.DeleteKnowledge(id); // Changed

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockKnowledgeRepo.Verify(repo => repo.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task UploadJsonl_ShouldReturnOk_WhenFileIsValid()
        {
            // Arrange
            var module = "test-module";
            var jsonlContent = "{\"contents\":[{\"role\":\"user\",\"parts\":[{\"text\":\"key1\"}]},{\"role\":\"model\",\"parts\":[{\"text\":\"value1\"}]}]}";
            var fileName = "test.jsonl";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonlContent));
            IFormFile file = new FormFile(stream, 0, stream.Length, "file", fileName);

            _mockDataKnowledgeService.Setup(s => s.UploadKnowledgeDataAsync(It.IsAny<Stream>(), module)).ReturnsAsync(1);

            // Act
            var result = await _controller.UploadJsonl(file, module);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            Assert.Contains("Successfully uploaded and processed 1 knowledge items", okResult.Value.ToString());
            _mockDataKnowledgeService.Verify(s => s.UploadKnowledgeDataAsync(It.IsAny<Stream>(), module), Times.Once);
        }

        [Fact]
        public async Task UploadJsonl_ShouldReturnBadRequest_WhenFileIsEmpty()
        {
            // Arrange
            var module = "test-module";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(""));
            IFormFile file = new FormFile(stream, 0, 0, "file", "empty.jsonl");

            // Act
            var result = await _controller.UploadJsonl(file, module);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
            Assert.Contains("No file uploaded.", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task UploadJsonl_ShouldReturnBadRequest_WhenFileIsNotJsonl()
        {
            // Arrange
            var module = "test-module";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes("not jsonl"));
            IFormFile file = new FormFile(stream, 0, stream.Length, "file", "test.txt");

            // Act
            var result = await _controller.UploadJsonl(file, module);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
            Assert.Contains("Invalid file type.", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task UploadJsonl_ShouldReturnBadRequest_WhenModuleIsMissing()
        {
            // Arrange
            var jsonlContent = "{}";
            var fileName = "test.jsonl";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonlContent));
            IFormFile file = new FormFile(stream, 0, stream.Length, "file", fileName);

            // Act
            var result = await _controller.UploadJsonl(file, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
            Assert.Contains("Module query parameter is required.", badRequestResult.Value.ToString());
        }

        [Fact]
        public async Task RegisterBlob_ShouldReturnOk_WhenRequestIsValid()
        {
            // Arrange
            var request = new RegisterBlobRequest
            {
                Key = "blob-key", // This Key is for the request DTO, not the entity
                Module = "blob-module",
                BlobUrl = "http://example.com/blob"
            };

            _mockDataKnowledgeService.Setup(s => s.UploadBlobKnowledgeAsync(request.Key, request.Module, request.BlobUrl)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.RegisterBlob(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            Assert.Contains("Successfully registered blob knowledge", okResult.Value.ToString());
            _mockDataKnowledgeService.Verify(s => s.UploadBlobKnowledgeAsync(request.Key, request.Module, request.BlobUrl), Times.Once);
        }

        [Fact]
        public async Task RegisterBlob_ShouldReturnBadRequest_WhenRequestIsInvalid()
        {
            // Arrange
            var request = new RegisterBlobRequest
            {
                Key = "", // Invalid
                Module = "blob-module",
                BlobUrl = "http://example.com/blob"
            };

            // Act
            var result = await _controller.RegisterBlob(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
            Assert.Contains("Key, Module, and BlobUrl are required.", badRequestResult.Value.ToString());
        }
    }
}