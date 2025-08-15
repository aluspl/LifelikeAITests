namespace AiAgent.Api.Domain.Common.Interfaces;

public interface IDataSeederService
{
    Task SeedApiKeysAsync();
    Task SeedAllDataAsync();
}
