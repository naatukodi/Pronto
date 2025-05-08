using Microsoft.EntityFrameworkCore;
using Pronto.ValuationApi.Data.Models;

namespace Pronto.ValuationApi.Data
{
    public class ValuationDbContext : DbContext
    {
        public DbSet<ValuationApi.Data.Models.Valuation> Valuations { get; set; }
        public DbSet<ValuationApi.Data.Models.ComponentType> ComponentTypes { get; set; }
        public DbSet<ValuationApi.Data.Models.WorkflowStepTemplate> WorkflowStepTemplates { get; set; }

        public ValuationDbContext(DbContextOptions<ValuationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Cosmos DB containers and partitioning
            modelBuilder.HasDefaultContainer("Valuations");

            modelBuilder.Entity<ValuationApi.Data.Models.Valuation>()
                .HasPartitionKey(v => v.AdjusterUserId) // Ensure AdjusterUserId is uniquely defined in the Valuation class
                .OwnsOne(v => v.Stakeholder)
                .OwnsOne(v => v.Applicant)
                .OwnsOne(v => v.VehicleDetails)
                .OwnsMany(v => v.Documents)
                .OwnsMany(v => v.Components)
                .OwnsOne(v => v.Summary)
                .OwnsMany(v => v.Workflow);

            modelBuilder.Entity<ValuationApi.Data.Models.ComponentType>()
                .ToContainer("ComponentTypes");

            modelBuilder.Entity<ValuationApi.Data.Models.WorkflowStepTemplate>()
                .ToContainer("WorkflowStepTemplates");
        }
    }
}