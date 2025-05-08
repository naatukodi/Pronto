using Microsoft.EntityFrameworkCore;
using Pronto.ValuationApi.Data;
using Pronto.ValuationApi.Data.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure Cosmos DB
var cosmosCfg = builder.Configuration.GetSection("Cosmos");
builder.Services.AddDbContext<ValuationDbContext>(opts =>
    opts.UseCosmos(
        builder.Configuration.GetConnectionString("CosmosDb") ?? throw new InvalidOperationException("CosmosDb connection string is not configured."),
        databaseName: cosmosCfg["Database"] ?? throw new InvalidOperationException("Cosmos database name is not configured.")));

// 2. Add your services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 3. Ensure database/containers exist and seed lookup data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ValuationDbContext>();

    // Create DB + containers
    await db.Database.EnsureCreatedAsync();

    // Seed ComponentTypes if empty
    var existingComponents = await db.ComponentTypes.ToListAsync();
    if (existingComponents.Count == 0)
    {
        db.ComponentTypes.AddRange(new[]
        {
            new ComponentType { Id = 1, Name = "ENGINE CONDITION", Description = "Status of engine internals" },
            new ComponentType { Id = 2, Name = "BRAKE SYSTEM",     Description = "Condition of brakes" },
            // … add all MASTER sheet items …
        });
        await db.SaveChangesAsync();
    }

    // Seed WorkflowStepTemplates if empty
    var existingSteps = await db.WorkflowStepTemplates.ToListAsync();
    if (existingSteps.Count == 0)
    {
        db.WorkflowStepTemplates.AddRange(new[]
        {
            new WorkflowStepTemplate { Id = 101, StepOrder = 1, StepName = "STEP - 1", Description = "Stake holder will raise the request in his portal" },
            new WorkflowStepTemplate { Id = 102, StepOrder = 2, StepName = "STEP - 2", Description = "PRONTO back end team will assign this case to AVO team" },
            new WorkflowStepTemplate { Id = 103, StepOrder = 3, StepName = "STEP - 3", Description = "PRONTO AVO team will complete the valuation in App" },
            new WorkflowStepTemplate { Id = 104, StepOrder = 4, StepName = "STEP - 4", Description = "PRONTO BACK END TEAM will carry out the data migration" },
            new WorkflowStepTemplate { Id = 105, StepOrder = 5, StepName = "STEP - 5", Description = "PRONTO QC TEAM will carry quality check on the final report" },
        });
        await db.SaveChangesAsync();
    }
}

// 4. Configure middleware
app.UseHttpsRedirection();
app.UseAuthorization();

// 5. Swagger (always on, or wrap in Dev check)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pronto API V1");
    c.RoutePrefix = string.Empty;
});

app.MapControllers();
app.Run();