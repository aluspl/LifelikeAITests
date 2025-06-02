using MongoDB.Driver;

namespace Api.Domain.Configuration;

public class MongoSettings
{
    public string ConnectionString { get; set; }
    public string DatabaseName { get; set; }
}