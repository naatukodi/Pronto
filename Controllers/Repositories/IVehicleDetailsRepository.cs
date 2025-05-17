// Pronto.ValuationApi.Data/Repositories/IVehicleDetailsRepository.cs
using System.Threading.Tasks;
using Pronto.ValuationApi.Data.Models;

namespace Pronto.ValuationApi.Data.Repositories
{
    public interface IVehicleDetailsRepository
    {
        Task<VehicleDetails?> GetAsync(string registrationNumber, string applicantContact);
        Task<VehicleDetails> UpsertAsync(
            string registrationNumber,
            string applicantContact,
            VehicleDetails details);
    }
}