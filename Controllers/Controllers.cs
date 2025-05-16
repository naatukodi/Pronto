using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pronto.ValuationApi.Data.Models;
using Pronto.ValuationApi.Data.Repositories;

namespace Pronto.ValuationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuationsController : ControllerBase
    {
        private readonly IValuationRepository _repo;
        public ValuationsController(IValuationRepository repo) => _repo = repo;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? status = null)
            => Ok(await _repo.GetAllAsync(status));

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var v = await _repo.GetByIdAsync(id);
            if (v == null) return NotFound();
            return Ok(v);
        }

        [HttpPost]
        public async Task<IActionResult> Post(Valuation v)
        {
            var created = await _repo.CreateAsync(v);
            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, Valuation v)
        {
            if (id != v.Id) return BadRequest();
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return NotFound();
            await _repo.UpdateAsync(v);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _repo.DeleteAsync(id);
            return NoContent();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ComponentTypesController : ControllerBase
    {
        private readonly Pronto.ValuationApi.Data.ValuationDbContext _context;
        public ComponentTypesController(Pronto.ValuationApi.Data.ValuationDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pronto.ValuationApi.Data.Models.ComponentType>>> GetComponentTypes()
        {
            return Ok(await _context.ComponentTypes.ToListAsync());
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class WorkflowStepTemplatesController : ControllerBase
    {
        private readonly Pronto.ValuationApi.Data.ValuationDbContext _context;
        public WorkflowStepTemplatesController(Pronto.ValuationApi.Data.ValuationDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pronto.ValuationApi.Data.Models.WorkflowStepTemplate>>> GetWorkflowTemplates()
        {
            return Ok(await _context.WorkflowStepTemplates.ToListAsync());
        }
    }
}