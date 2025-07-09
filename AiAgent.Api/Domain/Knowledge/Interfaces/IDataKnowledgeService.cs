namespace AiAgent.Api.Domain.Knowledge.Interfaces;

public interface IDataKnowledgeService
{
    Task<int> UploadKnowledgeDataAsync(Stream fileStream, string moduleString);
}
