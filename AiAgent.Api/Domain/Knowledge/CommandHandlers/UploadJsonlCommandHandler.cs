using AiAgent.Api.Infrastructure.CQRS.Interfaces;
using AiAgent.Api.Domain.Knowledge.Interfaces;
using AiAgent.Api.Domain.Knowledge.Commands;

namespace AiAgent.Api.Domain.Knowledge.CommandHandlers;

public class UploadJsonlCommandHandler(IDataKnowledgeService dataKnowledgeService) : ICommandHandler<UploadJsonlCommand>
{
    public async Task HandleAsync(UploadJsonlCommand command)
    {
        if (command.File == null || command.File.Length == 0)
        {
            throw new ArgumentException("No file uploaded.");
        }
        if (Path.GetExtension(command.File.FileName).ToLower() != ".jsonl")
        {
            throw new ArgumentException("Invalid file type. Please upload a .jsonl file.");
        }
        if (string.IsNullOrEmpty(command.Module))
        {
            throw new ArgumentException("Module query parameter is required.");
        }

        using (var stream = command.File.OpenReadStream())
        {
            await dataKnowledgeService.UploadKnowledgeDataAsync(stream, command.Module);
        }
    }
}