// Pronto.ValuationApi.Data/Repositories/ValuationRepository.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pronto.ValuationApi.Data;
using Pronto.ValuationApi.Data.Models;

namespace Pronto.ValuationApi.Data.Repositories
{
    public class ValuationRepository : IValuationRepository
    {
        private readonly ValuationDbContext _ctx;
        public ValuationRepository(ValuationDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Valuation>> GetAllAsync(string? status = null)
        {
            var query = _ctx.Valuations.AsQueryable();
            if (!string.IsNullOrEmpty(status))
                query = query.Where(v => v.Status == status);
            return await query.ToListAsync();
        }

        public async Task<Valuation?> GetByIdAsync(string id)
            => await _ctx.Valuations.SingleOrDefaultAsync(v => v.Id == id);

        public async Task<Valuation> CreateAsync(Valuation v)
        {
            // Compute composite partition key: RegistrationNumber:ApplicantContact
            var registration = v.VehicleDetails?.RegistrationNumber ?? string.Empty;
            var contact = v.Applicant?.Contact ?? string.Empty;
            v.PartitionKey = $"{registration}:{contact}";

            // Initialize audit fields
            v.Id = Guid.NewGuid().ToString();
            v.CreatedAt = v.UpdatedAt = DateTime.UtcNow;

            _ctx.Valuations.Add(v);
            await _ctx.SaveChangesAsync();
            return v;
        }

        public async Task UpdateAsync(Valuation v)
        {
            // Recalculate partition key in case registration or contact changed
            var registration = v.VehicleDetails?.RegistrationNumber ?? string.Empty;
            var contact = v.Applicant?.Contact ?? string.Empty;
            v.PartitionKey = $"{registration}:{contact}";
            v.UpdatedAt = DateTime.UtcNow;

            _ctx.Valuations.Update(v);
            await _ctx.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var valuation = await GetByIdAsync(id);
            if (valuation != null)
            {
                _ctx.Valuations.Remove(valuation);
                await _ctx.SaveChangesAsync();
            }
        }
    }
}
