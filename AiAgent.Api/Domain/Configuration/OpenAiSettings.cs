namespace AiAgent.Api.Domain.Configuration;

    public class OpenAiSettings
    {
        public string ApiKey { get; set; }
        public string Endpoint { get; set; }
        public string Deployment { get; set; }
        public string SystemChatMessage { get; set; }
    }
