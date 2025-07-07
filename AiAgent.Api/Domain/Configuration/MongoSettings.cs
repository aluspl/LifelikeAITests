using MongoDB.Driver;

namespace AiAgent.Api.Domain.Configuration;

public class MongoSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}