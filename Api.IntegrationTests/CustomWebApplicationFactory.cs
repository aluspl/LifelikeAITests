using Api;
using Api.Domain.AI.Interfaces;
using Api.Domain.Chat.Enums;
using Api.Domain.Chat.Interfaces;
using Api.Domain.Chat.Services;
using Api.Domain.Database.Entites;
using Api.Domain.Database.Interfaces;
using Api.Domain.Instructions.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MongoDB.Driver;

namespace Api.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing IAiService registrations
            var aiServiceDescriptors = services.Where(descriptor =>
                descriptor.ServiceType == typeof(IAiService)).ToList();
            foreach (var descriptor in aiServiceDescriptors)
            {
                services.Remove(descriptor);
            }

            // Remove existing IChatHistoryRepository and IInstructionService registrations
            var chatHistoryRepositoryDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IChatHistoryRepository));
            if (chatHistoryRepositoryDescriptor != null)
            {
                services.Remove(chatHistoryRepositoryDescriptor);
            }

            var instructionServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IInstructionService));
            if (instructionServiceDescriptor != null)
            {
                services.Remove(instructionServiceDescriptor);
            }

            // Remove existing IMongoClient and IMongoDatabase registrations
            var mongoClientDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IMongoClient));
            if (mongoClientDescriptor != null)
            {
                services.Remove(mongoClientDescriptor);
            }

            var mongoDatabaseDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IMongoDatabase));
            if (mongoDatabaseDescriptor != null)
            {
                services.Remove(mongoDatabaseDescriptor);
            }

            // Add our mocked IAiService
            var mockAiService = new Mock<IAiService>();
            mockAiService.Setup(s => s.ProcessAsync(It.IsAny<string>(), It.IsAny<string>()))
                         .ReturnsAsync("Mocked AI Response");

            services.AddKeyedTransient<IAiService, IAiService>(
                AiProvider.Gemini.ToString().ToLower(),
                (sp, key) => mockAiService.Object);
            services.AddKeyedTransient<IAiService, IAiService>(
                AiProvider.OpenAi.ToString().ToLower(),
                (sp, key) => mockAiService.Object);

            // Add our mocked IMongoClient and IMongoDatabase
            var mockMongoClient = new Mock<IMongoClient>();
            var mockMongoDatabase = new Mock<IMongoDatabase>();
            var mockChatHistoryCollection = new Mock<IMongoCollection<ChatHistoryEntity>>();
            var mockInstructionCollection = new Mock<IMongoCollection<InstructionEntity>>();

            mockMongoClient.Setup(client => client.GetDatabase(It.IsAny<string>(), null))
                           .Returns(mockMongoDatabase.Object);
            mockMongoDatabase.Setup(db => db.GetCollection<ChatHistoryEntity>(It.IsAny<string>(), null))
                             .Returns(mockChatHistoryCollection.Object);
            mockMongoDatabase.Setup(db => db.GetCollection<InstructionEntity>(It.IsAny<string>(), null))
                             .Returns(mockInstructionCollection.Object);

            mockChatHistoryCollection.Setup(c => c.InsertOneAsync(It.IsAny<ChatHistoryEntity>(), null, CancellationToken.None))
                                     .Returns(Task.CompletedTask);
            mockInstructionCollection.Setup(c => c.InsertOneAsync(It.IsAny<InstructionEntity>(), null, CancellationToken.None))
                                     .Returns(Task.CompletedTask);

            services.AddSingleton<IMongoClient>(_ => mockMongoClient.Object);
            services.AddSingleton<IMongoDatabase>(_ => mockMongoDatabase.Object);

            // Add our mocked IChatHistoryRepository and IInstructionService
            var mockChatHistoryRepository = new Mock<IChatHistoryRepository>();
            var mockInstructionService = new Mock<IInstructionService>();

            services.AddScoped<IChatHistoryRepository>(_ => mockChatHistoryRepository.Object);
            services.AddScoped<IInstructionService>(_ => mockInstructionService.Object);

            // Register ChatService with mocked dependencies
            services.AddScoped<IChatService, ChatService>(provider =>
            {
                Func<string, IAiService> aiServiceFactory = key => provider.GetRequiredKeyedService<IAiService>(key);
                return new ChatService(aiServiceFactory, provider.GetRequiredService<IChatHistoryRepository>(), provider.GetRequiredService<IInstructionService>());
            });
        });
    }
}