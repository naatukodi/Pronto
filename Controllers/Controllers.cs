using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Pronto.ValuationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuationsController : ControllerBase
    {
        private readonly Pronto.ValuationApi.Data.ValuationDbContext _context;
        public ValuationsController(Pronto.ValuationApi.Data.ValuationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pronto.ValuationApi.Data.Models.Valuation>>> GetValuations([FromQuery]string adjusterUserId, [FromQuery]string status)
        {
            var query = _context.Valuations.AsQueryable();
            if (!string.IsNullOrEmpty(adjusterUserId))
                query = query.Where(v => v.AdjusterUserId == adjusterUserId);
            if (!string.IsNullOrEmpty(status))
                query = query.Where(v => v.Status == status);
            return Ok(await query.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pronto.ValuationApi.Data.Models.Valuation>> GetValuation(string id)
        {
            var valuation = await _context.Valuations.SingleOrDefaultAsync(v => v.Id == id);
            if (valuation == null) return NotFound();
            return Ok(valuation);
        }

        [HttpPost]
        public async Task<ActionResult<Pronto.ValuationApi.Data.Models.Valuation>> CreateValuation(Pronto.ValuationApi.Data.Models.Valuation valuation)
        {
            valuation.Id = Guid.NewGuid().ToString();
            valuation.CreatedAt = DateTime.UtcNow;
            valuation.UpdatedAt = DateTime.UtcNow;
            if (_context == null) throw new InvalidOperationException("Database context is not initialized.");
            _context.Valuations.Add(valuation);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetValuation), new { id = valuation.Id }, valuation);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateValuation(string id, Pronto.ValuationApi.Data.Models.Valuation valuation)
        {
            if (id != valuation.Id) return BadRequest();
            var exists = await _context.Valuations.AsNoTracking().AnyAsync(v => v.Id == id);
            if (!exists) return NotFound();
            valuation.UpdatedAt = DateTime.UtcNow;
            _context.Valuations.Update(valuation);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteValuation(string id)
        {
            var valuation = await _context.Valuations.SingleOrDefaultAsync(v => v.Id == id);
            if (valuation == null) return NotFound();
            _context.Valuations.Remove(valuation);
            await _context.SaveChangesAsync();
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