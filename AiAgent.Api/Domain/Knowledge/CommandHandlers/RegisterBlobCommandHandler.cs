using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Knowledge.Interfaces;
using AiAgent.Api.Domain.Knowledge.Commands;

namespace AiAgent.Api.Domain.Knowledge.CommandHandlers;

public class RegisterBlobCommandHandler(IDataKnowledgeService dataKnowledgeService)
    : ICommandHandler<RegisterBlobCommand>
{
    public async Task HandleAsync(RegisterBlobCommand command)
    {
        if (string.IsNullOrEmpty(command.Request.Key) || string.IsNullOrEmpty(command.Request.Module) || string.IsNullOrEmpty(command.Request.BlobUrl))
        {
            throw new ArgumentException("Key, Module, and BlobUrl are required.");
        }

        await dataKnowledgeService.UploadBlobKnowledgeAsync(command.Request.Key, command.Request.Module, command.Request.BlobUrl);
    }
}