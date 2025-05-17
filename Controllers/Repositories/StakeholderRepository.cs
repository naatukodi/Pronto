// Data/Repositories/StakeholderRepository.cs
using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Pronto.ValuationApi.Data.Models;

namespace Pronto.ValuationApi.Data.Repositories
{
    public class StakeholderRepository : IStakeholderRepository
    {
        private readonly ValuationDbContext _ctx;
        private readonly BlobContainerClient _container;

        public StakeholderRepository(
            ValuationDbContext ctx,
            BlobServiceClient blobService,
            IOptions<StorageSettings> storageOptions)
        {
            _ctx = ctx;
            var settings = storageOptions.Value;
            if (string.IsNullOrWhiteSpace(settings.ConnectionString))
                throw new InvalidOperationException("Storage:ConnectionString is not configured.");

            // Initialize the container client
            _container = blobService.GetBlobContainerClient(settings.ContainerName);
            _container.CreateIfNotExists();
        }

        private string ComposePK(string regNo, string contact)
            => $"{regNo}:{contact}";

        public async Task<Stakeholder> UpsertAsync(string regNo, string contact, Stakeholder stakeholder)
        {
            var pk = ComposePK(regNo, contact);
            var valuation = await _ctx.Valuations
                .SingleOrDefaultAsync(v => v.PartitionKey == pk);

            if (valuation == null)
            {
                valuation = new Valuation
                {
                    PartitionKey = pk,
                    VehicleDetails = new VehicleDetails { RegistrationNumber = regNo },
                    Applicant = new Applicant { Contact = contact },
                    Stakeholder = stakeholder
                };
                _ctx.Valuations.Add(valuation);
            }
            else
            {
                valuation.Stakeholder = stakeholder;
                _ctx.Valuations.Update(valuation);
            }

            await _ctx.SaveChangesAsync();
            return valuation.Stakeholder;
        }

        public async Task DeleteAsync(string regNo, string contact)
        {
            var pk = ComposePK(regNo, contact);
            var valuation = await _ctx.Valuations
                .SingleOrDefaultAsync(v => v.PartitionKey == pk);
            if (valuation != null)
            {
                // clear only stakeholder
                valuation.Stakeholder = null;
                _ctx.Valuations.Update(valuation);
                await _ctx.SaveChangesAsync();
            }
        }

        public async Task<DocumentUpload> AddDocumentAsync(
                string regNo,
                string contact,
                string type,
                Stream fileStream,
                string contentType)
        {
            var pk = ComposePK(regNo, contact);
            var valuation = await _ctx.Valuations
                .SingleOrDefaultAsync(v => v.PartitionKey == pk);

            if (valuation == null)
                throw new KeyNotFoundException($"No valuation for {pk}");

            // Upload to Blob
            var blobName = $"{pk}/{Guid.NewGuid()}.dat";
            var blob = _container.GetBlobClient(blobName);
            await blob.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = contentType });

            // Create DocumentUpload
            var doc = new DocumentUpload
            {
                Type = type,
                FilePath = blob.Uri.ToString(),
                UploadedAt = DateTime.UtcNow
            };

            // Ensure vehicleDetails exists
            var vd = valuation.VehicleDetails ??= new VehicleDetails { RegistrationNumber = regNo };
            vd.Documents.Add(doc);

            // Save
            _ctx.Valuations.Update(valuation);
            await _ctx.SaveChangesAsync();

            return doc;
        }
    }
}