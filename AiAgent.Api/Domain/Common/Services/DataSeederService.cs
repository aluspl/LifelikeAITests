using AiAgent.Api.Domain.Common.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using Microsoft.Extensions.Options;
using AiAgent.Api.Domain.Configuration;

namespace AiAgent.Api.Domain.Common.Services;

public class DataSeederService : IDataSeederService
{
    private readonly IApiKeyRepository _apiKeyRepository;
    private readonly ApiKeySettings _apiKeySettings;

    public DataSeederService(IKnowledgeRepository knowledgeRepository, IApiKeyRepository apiKeyRepository, IOptions<ApiKeySettings> apiKeyOptions)
    {
        _apiKeyRepository = apiKeyRepository;
        _apiKeySettings = apiKeyOptions.Value;
    }

    public async Task SeedApiKeysAsync()
    {
        var existingKey = await _apiKeyRepository.GetByKeyAsync(_apiKeySettings.Key);
        if (existingKey == null)
        {
            var apiKey = new ApiKey
            {
                Key = _apiKeySettings.Key,
                Owner = _apiKeySettings.Owner,
                Created = DateTime.UtcNow,
                Expires = null // Never expires
            };
            await _apiKeyRepository.InsertAsync(apiKey);
        }
    }
}
