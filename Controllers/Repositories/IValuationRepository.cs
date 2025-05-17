// Pronto.ValuationApi.Data/Repositories/IValuationRepository.cs
using Pronto.ValuationApi.Data.Models;

namespace Pronto.ValuationApi.Data.Repositories;
public interface IValuationRepository
{
    Task<Stakeholder> GetAsync(string registrationNumber, string applicantContact);
    Task<IEnumerable<Valuation>> GetAllAsync(string? status = null);
    Task<Valuation?> GetByIdAsync(string id);
    Task<Valuation> CreateAsync(Valuation v);
    Task UpdateAsync(Valuation v);
    Task DeleteAsync(string id);
}
