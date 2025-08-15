using System;
using System.Collections.Generic;
using AiAgent.Api.Domain.Database.Entites;

namespace AiAgent.Api.Domain.Database.Entites
{
    public class ExecutionLogEntity : BaseEntity
    {
        public string AgentId { get; set; }
        public string CorrelationId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<StepExecutionResult> StepResults { get; set; } = new List<StepExecutionResult>();
    }
}
