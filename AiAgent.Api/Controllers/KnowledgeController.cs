using AiAgent.Api.Domain.Database.Entites;
using AiAgent.Api.Domain.Database.Interfaces;
using AiAgent.Api.Domain.Knowledge.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using AiAgent.Api.Domain.Knowledge.Enums;

namespace AiAgent.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KnowledgeController : ControllerBase
    {
        private readonly IKnowledgeRepository _knowledgeRepository;
        private readonly IDataKnowledgeService _dataKnowledgeService;

        public KnowledgeController(IKnowledgeRepository knowledgeRepository, IDataKnowledgeService dataKnowledgeService)
        {
            _knowledgeRepository = knowledgeRepository;
            _dataKnowledgeService = dataKnowledgeService;
        }

        // GET: api/Knowledge
        [HttpGet]
        public async Task<ActionResult<IEnumerable<KnowledgeEntity>>> GetKnowledge()
        {
            var knowledge = await _knowledgeRepository.GetAllAsync();
            return Ok(knowledge);
        }

        // GET: api/Knowledge/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<KnowledgeEntity>> GetKnowledge(string id)
        {
            var knowledge = await _knowledgeRepository.GetByIdAsync(id);
            if (knowledge == null)
            {
                return NotFound();
            }
            return Ok(knowledge);
        }

        // POST: api/Knowledge
        [HttpPost]
        public async Task<ActionResult<KnowledgeEntity>> CreateKnowledge(KnowledgeEntity knowledge)
        {
            await _knowledgeRepository.InsertAsync(knowledge);
            return CreatedAtAction(nameof(GetKnowledge), new { id = knowledge.Id }, knowledge);
        }

        // PUT: api/Knowledge/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateKnowledge(string id, KnowledgeEntity knowledge)
        {
            if (id != knowledge.Id)
            {
                return BadRequest();
            }
            await _knowledgeRepository.UpdateAsync(knowledge);
            return NoContent();
        }

        // DELETE: api/Knowledge/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteKnowledge(string id)
        {
            await _knowledgeRepository.DeleteAsync(id);
            return NoContent();
        }

        // POST: api/Knowledge/upload-jsonl
        [HttpPost("upload-jsonl")]
        public async Task<IActionResult> UploadJsonl(IFormFile file, [FromQuery] string module)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            if (Path.GetExtension(file.FileName).ToLower() != ".jsonl")
            {
                return BadRequest("Invalid file type. Please upload a .jsonl file.");
            }
            if (string.IsNullOrEmpty(module))
            {
                return BadRequest("Module query parameter is required.");
            }

            using (var stream = file.OpenReadStream())
            {
                var count = await _dataKnowledgeService.UploadKnowledgeDataAsync(stream, module);
                return Ok($"Successfully uploaded and processed {count} knowledge items for module {module}.");
            }
        }

        // POST: api/Knowledge/register-blob
        [HttpPost("register-blob")]
        public async Task<IActionResult> RegisterBlob([FromBody] RegisterBlobRequest request)
        {
            if (string.IsNullOrEmpty(request.Key) || string.IsNullOrEmpty(request.Module) || string.IsNullOrEmpty(request.BlobUrl))
            {
                return BadRequest("Key, Module, and BlobUrl are required.");
            }

            await _dataKnowledgeService.UploadBlobKnowledgeAsync(request.Key, request.Module, request.BlobUrl);
            return Ok($"Successfully registered blob knowledge for key '{request.Key}' in module '{request.Module}'.");
        }
    }

    public class RegisterBlobRequest
    {
        public string Key { get; set; }
        public string Module { get; set; }
        public string BlobUrl { get; set; }
    }
}