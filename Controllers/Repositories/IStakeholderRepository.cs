// Data/Repositories/IStakeholderRepository.cs
using System.Threading.Tasks;
using Pronto.ValuationApi.Data.Models;

namespace Pronto.ValuationApi.Data.Repositories
{
    public interface IStakeholderRepository
    {
        Task<Stakeholder> GetAsync(string registrationNumber, string applicantContact);
        Task<Stakeholder> UpsertAsync(string registrationNumber, string applicantContact, Stakeholder stakeholder);
        Task DeleteAsync(string registrationNumber, string applicantContact);
        Task<DocumentUpload> AddDocumentAsync(
        string registrationNumber,
        string applicantContact,
        string type,
        Stream content,
        string contentType);
    }
}