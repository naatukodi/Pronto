// Pronto.ValuationApi.Controllers/VehicleDetailsController.cs
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pronto.ValuationApi.Data.Models;
using Pronto.ValuationApi.Data.Repositories;

namespace Pronto.ValuationApi.Controllers
{
    [ApiController]
    [Route("api/vehicle-details")]
    public class VehicleDetailsController : ControllerBase
    {
        private readonly IVehicleDetailsRepository _repo;
        public VehicleDetailsController(IVehicleDetailsRepository repo) => _repo = repo;

        [HttpGet("{regNo}/{contact}")]
        public async Task<IActionResult> Get(string regNo, string contact)
        {
            var details = await _repo.GetAsync(regNo, contact);
            if (details == null) return NotFound();
            return Ok(details);
        }

        [HttpPut("{regNo}/{contact}")]
        public async Task<IActionResult> Upsert(
            string regNo,
            string contact,
            [FromBody] VehicleDetails details)
        {
            var result = await _repo.UpsertAsync(regNo, contact, details);
            return Ok(result);
        }
    }
}
