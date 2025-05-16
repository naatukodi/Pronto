// Data/ValuationDbContext.cs
using Microsoft.EntityFrameworkCore;
using Pronto.ValuationApi.Data.Models;

namespace Pronto.ValuationApi.Data
{
    public class ValuationDbContext : DbContext
    {
        public DbSet<Valuation> Valuations { get; set; }
        public DbSet<ComponentType> ComponentTypes { get; set; }
        public DbSet<WorkflowStepTemplate> WorkflowStepTemplates { get; set; }

        public ValuationDbContext(DbContextOptions<ValuationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultContainer("Valuations");

            modelBuilder.Entity<Valuation>(b =>
            {
                b.HasPartitionKey(v => v.PartitionKey);
                b.OwnsOne(v => v.Stakeholder);
                b.OwnsOne(v => v.Applicant);
                b.OwnsOne(v => v.VehicleDetails);
                b.OwnsMany(v => v.Documents);
                b.OwnsMany(v => v.Components);
                b.OwnsOne(v => v.Summary);
                b.OwnsMany(v => v.Workflow);
            });

            modelBuilder.Entity<ComponentType>()
                .ToContainer("ComponentTypes");

            modelBuilder.Entity<WorkflowStepTemplate>()
                .ToContainer("WorkflowStepTemplates");
        }
    }
}