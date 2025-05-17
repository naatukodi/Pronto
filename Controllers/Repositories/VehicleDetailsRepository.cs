// Pronto.ValuationApi.Data/Repositories/VehicleDetailsRepository.cs
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pronto.ValuationApi.Data;
using Pronto.ValuationApi.Data.Models;

namespace Pronto.ValuationApi.Data.Repositories
{
    public class VehicleDetailsRepository : IVehicleDetailsRepository
    {
        private readonly ValuationDbContext _ctx;
        public VehicleDetailsRepository(ValuationDbContext ctx) => _ctx = ctx;

        private static string ComposePartitionKey(string regNo, string contact)
            => $"{regNo}:{contact}";

        public async Task<VehicleDetails?> GetAsync(string regNo, string contact)
        {
            var pk = ComposePartitionKey(regNo, contact);
            var valuation = await _ctx.Valuations
                .AsNoTracking()
                .SingleOrDefaultAsync(v => v.PartitionKey == pk);
            return valuation?.VehicleDetails;
        }

        public async Task<VehicleDetails> UpsertAsync(
            string regNo,
            string contact,
            VehicleDetails details)
        {
            var pk = ComposePartitionKey(regNo, contact);
            var valuation = await _ctx.Valuations
                .SingleOrDefaultAsync(v => v.PartitionKey == pk);

            if (valuation == null)
            {
                // Create a new valuation stub
                valuation = new Valuation
                {
                    PartitionKey = pk,
                    Applicant = new Applicant { Contact = contact },
                    VehicleDetails = details
                };
                _ctx.Valuations.Add(valuation);
            }
            else
            {
                valuation.VehicleDetails = details;
                _ctx.Valuations.Update(valuation);
            }

            await _ctx.SaveChangesAsync();
            return details;
        }
    }
}
