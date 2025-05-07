using Microsoft.EntityFrameworkCore;
using Pronto.ValuationApi.Data.Models;

namespace Pronto.ValuationApi.Data
{
    public class ProntoDbContext : DbContext
    {
        public ProntoDbContext(DbContextOptions<ProntoDbContext> opts)
          : base(opts) { }

        public DbSet<Models.Valuation> Valuations { get; set; }
        public DbSet<Models.ComponentType> ComponentTypes { get; set; }
        public DbSet<Models.WorkflowStepTemplate> WorkflowStepTemplates { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultContainer("Valuations");
            builder.Entity<Models.Valuation>()
              .HasPartitionKey(v => v.AdjusterUserId)
              .OwnsOne(v => v.Stakeholder)
              .OwnsOne(v => v.Applicant)
              .OwnsOne(v => v.VehicleDetails)
              .OwnsMany(v => v.Documents)
              .OwnsMany(v => v.Components)
              .OwnsOne(v => v.Summary)
              .OwnsMany(v => v.Workflow);

            builder.Entity<Models.ComponentType>().ToContainer("ComponentTypes");
            builder.Entity<Models.WorkflowStepTemplate>().ToContainer("WorkflowStepTemplates");
        }
    }
}
