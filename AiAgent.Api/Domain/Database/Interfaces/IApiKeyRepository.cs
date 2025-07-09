using AiAgent.Api.Domain.Database.Entites;

namespace AiAgent.Api.Domain.Database.Interfaces;

public interface IApiKeyRepository : IRepository<ApiKey>
{
    Task<ApiKey?> GetByKeyAsync(string key);
}
