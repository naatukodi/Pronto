// Controllers/StakeholderController.cs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronto.ValuationApi.Data.Models;
using Pronto.ValuationApi.Data.Repositories;

namespace Pronto.ValuationApi.Controllers
{
    [ApiController]
    [Route("api/stakeholders")]
    public class StakeholderController : ControllerBase
    {
        private readonly IStakeholderRepository _repo;
        public StakeholderController(IStakeholderRepository repo) => _repo = repo;

        [HttpGet("{regNo}/{contact}")]
        public async Task<IActionResult> Get(string regNo, string contact)
        {
            var stakeholder = await _repo.GetAsync(regNo, contact);
            if (stakeholder == null) return NotFound();
            return Ok(stakeholder);
        }

        [HttpPut("{regNo}/{contact}")]
        public async Task<IActionResult> Upsert(
            string regNo,
            string contact,
            [FromBody] Stakeholder stakeholder)
        {
            var result = await _repo.UpsertAsync(regNo, contact, stakeholder);
            return Ok(result);
        }

        [HttpDelete("{regNo}/{contact}")]
        public async Task<IActionResult> Delete(string regNo, string contact)
        {
            await _repo.DeleteAsync(regNo, contact);
            return NoContent();
        }

        /// <summary>
        /// Upload a document (RC, Insurance, Others) for this stakeholder's vehicle.
        /// </summary>
        [HttpPost("{regNo}/{contact}/documents")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadDocument(
            string regNo,
            string contact,
            [FromForm] string type,
            [FromForm] IFormFile file)
        {
            if (file.Length == 0)
                return BadRequest("Empty file.");

            using var stream = file.OpenReadStream();
            var doc = await _repo.AddDocumentAsync(
                regNo,
                contact,
                type,
                stream,
                file.ContentType);

            return Ok(doc);
        }
    }
}