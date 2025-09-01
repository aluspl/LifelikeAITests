using AiAgent.Api.Domain.Common.Interfaces;

namespace AiAgent.Api.Domain.Knowledge.Interfaces;

public interface IDataKnowledgeService : IService
{
    Task<int> UploadKnowledgeDataAsync(Stream fileStream, string moduleString);
    Task UploadBlobKnowledgeAsync(string key, string module, string blobUrl);
    Task PopulateKnowledgeFromDictionaryAsync(string key, string module, Dictionary<string, string> data);
}