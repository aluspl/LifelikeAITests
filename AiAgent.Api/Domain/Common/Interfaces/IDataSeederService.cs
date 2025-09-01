namespace AiAgent.Api.Domain.Common.Interfaces;

public interface IDataSeederService : IService
{
    Task SeedApiKeysAsync();
    Task SeedAllDataAsync();
}
