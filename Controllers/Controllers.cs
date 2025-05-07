using Microsoft.AspNetCore.Mvc;
using Pronto.ValuationApi.Data;
using Microsoft.EntityFrameworkCore;


namespace Pronto.ValuationApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ValuationsController : ControllerBase
    {
        private readonly Data.ProntoDbContext _context;
        public ValuationsController(Data.ProntoDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Data.Models.Valuation>>> GetValuations(
            [FromQuery] string adjusterUserId,
            [FromQuery] string status)
        {
            var query = _context.Valuations.AsQueryable();
            if (!string.IsNullOrEmpty(adjusterUserId))
                query = query.Where(v => v.AdjusterUserId == adjusterUserId);
            if (!string.IsNullOrEmpty(status))
                query = query.Where(v => v.Status == status);
            var results = await query.ToListAsync();
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Data.Models.Valuation>> GetValuation(string id)
        {
            var valuation = await _context.Valuations
                .SingleOrDefaultAsync(v => v.Id == id);
            if (valuation == null)
                return NotFound();
            return Ok(valuation);
        }

        [HttpPost]
        public async Task<ActionResult<Data.Models.Valuation>> CreateValuation(Data.Models.Valuation valuation)
        {
            valuation.Id = Guid.NewGuid().ToString();
            valuation.CreatedAt = DateTime.UtcNow;
            valuation.UpdatedAt = DateTime.UtcNow;
            _context.Valuations.Add(valuation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetValuation), new { id = valuation.Id }, valuation);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateValuation(string id, Data.Models.Valuation valuation)
        {
            if (id != valuation.Id)
                return BadRequest();

            var exists = await _context.Valuations
                .AsNoTracking()
                .AnyAsync(v => v.Id == id);
            if (!exists)
                return NotFound();

            valuation.UpdatedAt = DateTime.UtcNow;
            _context.Valuations.Update(valuation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteValuation(string id)
        {
            var valuation = await _context.Valuations
                .SingleOrDefaultAsync(v => v.Id == id);
            if (valuation == null)
                return NotFound();

            _context.Valuations.Remove(valuation);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class ComponentTypesController : ControllerBase
    {
        private readonly Data.ProntoDbContext _context;
        public ComponentTypesController(Data.ProntoDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Data.Models.ComponentType>>> GetComponentTypes()
        {
            var list = await _context.ComponentTypes.ToListAsync();
            return Ok(list);
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class WorkflowStepTemplatesController : ControllerBase
    {
        private readonly Data.ProntoDbContext _context;
        public WorkflowStepTemplatesController(Data.ProntoDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Data.Models.WorkflowStepTemplate>>> GetWorkflowTemplates()
        {
            var list = await _context.WorkflowStepTemplates.ToListAsync();
            return Ok(list);
        }
    }
}