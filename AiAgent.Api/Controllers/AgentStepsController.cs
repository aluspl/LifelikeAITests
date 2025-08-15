using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AiAgent.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgentStepsController : ControllerBase
    {
        private readonly IAgentStepRepository _agentStepRepository;

        public AgentStepsController(IAgentStepRepository agentStepRepository)
        {
            _agentStepRepository = agentStepRepository;
        }

        // GET: api/AgentSteps
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AgentStepEntity>>> GetAgentSteps()
        {
            var steps = await _agentStepRepository.GetAllAsync();
            return Ok(steps);
        }

        // GET: api/AgentSteps/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<AgentStepEntity>> GetAgentStep(string id)
        {
            var step = await _agentStepRepository.GetByIdAsync(id);
            if (step == null)
            {
                return NotFound();
            }
            return Ok(step);
        }

        // POST: api/AgentSteps
        [HttpPost]
        public async Task<ActionResult<AgentStepEntity>> CreateAgentStep(AgentStepEntity step)
        {
            await _agentStepRepository.InsertAsync(step);
            return CreatedAtAction(nameof(GetAgentStep), new { id = step.Id }, step);
        }

        // PUT: api/AgentSteps/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAgentStep(string id, AgentStepEntity step)
        {
            if (id != step.Id)
            {
                return BadRequest();
            }
            await _agentStepRepository.UpdateAsync(step);
            return NoContent();
        }

        // DELETE: api/AgentSteps/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAgentStep(string id)
        {
            await _agentStepRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
