using AiAgent.Api.Domain.Common.Interfaces;
using System.Threading.Tasks;

namespace Api.IntegrationTests
{
    // This seeder does nothing. It's used in the test environment to override the 
    // production seeder, preventing it from running during integration tests.
    public class TestScopedDataSeeder : IDataSeederService
    {
        public Task SeedAllDataAsync()
        {
            return Task.CompletedTask;
        }

        public Task SeedApiKeysAsync()
        {
            return Task.CompletedTask;
        }
    }
}