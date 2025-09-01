namespace AiAgent.Api.Domain.Database.Entites
{
    public class StepExecutionResult
    {
        public string StepName { get; set; }
        public string Input { get; set; }
        public string Output { get; set; }
        public string Content { get; set; }
        public bool WasCached { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
