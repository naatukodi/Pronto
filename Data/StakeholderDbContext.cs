using Microsoft.EntityFrameworkCore;
using Pronto.ValuationApi.Data.Models;

namespace Pronto.ValuationApi.Data;

public class StakeholderDbContext : DbContext
{
    public DbSet<Stakeholder> Stakeholders { get; set; }

    public StakeholderDbContext(DbContextOptions<StakeholderDbContext> opts)
        : base(opts) { }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasDefaultContainer("Stakeholders");
        b.Entity<Stakeholder>(e =>
        {
            e.HasPartitionKey(s => s.id);
            e.OwnsOne(s => s.Applicant, a =>
            {
                a.OwnsOne(x => x.VehicleDetails, vd =>
                {
                    vd.OwnsMany(x => x.Documents, d =>
                    {
                        d.OwnsMany(x => x.Components, c =>
                        {
                            c.OwnsOne(x => x.Summary, sum =>
                            {
                                sum.OwnsMany(x => x.Workflow);
                            });
                        });
                    });
                });
            });
        });
    }
}
