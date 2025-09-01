namespace AiAgent.Api.Domain.Database.Entites
{
    public class ExecutionLogEntity : BaseEntity
    {
        public Guid AgentId { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public List<StepExecutionResult> StepResults { get; set; } = [];
    }
}   
