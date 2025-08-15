using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Agents.Orchestration;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AiAgent.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgentsController : ControllerBase
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IAgentOrchestratorService _agentOrchestratorService;

        public AgentsController(IAgentRepository agentRepository, IAgentOrchestratorService agentOrchestratorService)
        {
            _agentRepository = agentRepository;
            _agentOrchestratorService = agentOrchestratorService;
        }

        // GET: api/Agents
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AgentEntity>>> GetAgents()
        {
            var agents = await _agentRepository.GetAllAsync();
            return Ok(agents);
        }

        // GET: api/Agents/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AgentEntity>> GetAgent(string id)
        {
            var agent = await _agentRepository.GetByIdAsync(id);
            if (agent == null)
            {
                return NotFound();
            }
            return Ok(agent);
        }

        // POST: api/Agents
        [HttpPost]
        public async Task<ActionResult<AgentEntity>> CreateAgent(AgentEntity agent)
        {
            await _agentRepository.InsertAsync(agent);
            return CreatedAtAction(nameof(GetAgent), new { id = agent.Id }, agent);
        }

        // PUT: api/Agents/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAgent(string id, AgentEntity agent)
        {
            if (id != agent.Id)
            {
                return BadRequest();
            }
            await _agentRepository.UpdateAsync(agent);
            return NoContent();
        }

        // DELETE: api/Agents/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgent(string id)
        {
            await _agentRepository.DeleteAsync(id);
            return NoContent();
        }

        // POST: api/Agents/{id}/execute
        [HttpPost("{id}/execute")]
        public async Task<ActionResult<string>> ExecuteAgent(string id, [FromBody] List<Dictionary<string, string>> parameters)
        {
            try
            {
                // Convert the list of dictionaries into a single JSON string for the orchestrator
                // Example: [{"team1":"mandrakes"}, {"language":"pl"}] -> {"team1":"mandrakes", "language":"pl"}
                var initialInputDict = new Dictionary<string, string>();
                foreach (var paramDict in parameters)
                {
                    foreach (var entry in paramDict)
                    {
                        initialInputDict[entry.Key] = entry.Value;
                    }
                }
                var initialInputJson = System.Text.Json.JsonSerializer.Serialize(initialInputDict);

                var result = await _agentOrchestratorService.ExecuteAgent(id, initialInputJson);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
