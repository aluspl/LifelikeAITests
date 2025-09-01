using AiAgent.Api.Domain.Common.Interfaces;
using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using Microsoft.Extensions.Options;
using AiAgent.Api.Domain.Configuration;

namespace AiAgent.Api.Domain.Common.Services
{
    public class DataSeederService(
        IApiKeyRepository apiKeyRepository,
        IOptions<ApiKeySettings> apiKeyOptions
        )
        : IDataSeederService
    {
        private readonly ApiKeySettings _apiKeySettings = apiKeyOptions.Value;

        public async Task SeedAllDataAsync()
        {
            await SeedApiKeysAsync();
        }

        public async Task SeedApiKeysAsync()
        {
            var existingKey = await apiKeyRepository.GetByKeyAsync(_apiKeySettings.Key); // Use GetByKeyAsync
            if (existingKey == null)
            {
                var apiKey = new ApiKey
                {
                    Key = _apiKeySettings.Key, // Use Key
                    Owner = _apiKeySettings.Owner,
                    Created = DateTime.UtcNow,
                    Expires = null // Never expires
                };
                await apiKeyRepository.InsertAsync(apiKey);
            }
        }
    }
}