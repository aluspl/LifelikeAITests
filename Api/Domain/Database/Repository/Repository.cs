using Api.Domain.Configuration;
using Api.Domain.Database.Entites;
using Api.Domain.Database.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Api.Domain.Database.Repository;

public class Repository : IRepository
{
    protected readonly IMongoDatabase Database;

    public Repository(IOptions<MongoSettings> mongoSettings)
    {
        Database = MongoDatabase(mongoSettings);
    }
    
    protected static IMongoDatabase MongoDatabase(IOptions<MongoSettings> mongoSettings)
    {
        var client = new MongoClient(mongoSettings.Value.ConnectionString);
        var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
        return database;
    }
}