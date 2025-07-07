namespace AiAgent.Api.Domain.Configuration;

// Define GeminiSettings if it does not exist elsewhere
public class GeminiSettings
{
    public string ApiKey { get; set; }
    public string ModelId { get; set; }
    public string ProjectId { get; set; }
    public string Location { get; set; }
    public string Publisher { get; set; }
}
