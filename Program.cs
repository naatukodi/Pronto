// Program.cs
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Azure.Storage.Blobs;
using Pronto.ValuationApi.Data;
using Pronto.ValuationApi.Data.Models;
using Pronto.ValuationApi.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Bind and validate CosmosDb settings
var cosmosSection = builder.Configuration.GetSection("CosmosDb");
if (!cosmosSection.Exists())
    throw new InvalidOperationException("CosmosDb section is not configured in appsettings.json.");

builder.Services.Configure<CosmosDbSettings>(cosmosSection);
var cosmosSettings = cosmosSection.Get<CosmosDbSettings>() ?? throw new InvalidOperationException("CosmosDb settings must be provided.");
if (string.IsNullOrWhiteSpace(cosmosSettings.Account) ||
    string.IsNullOrWhiteSpace(cosmosSettings.Key) ||
    string.IsNullOrWhiteSpace(cosmosSettings.DatabaseName))
{
    throw new InvalidOperationException("CosmosDb settings (Account, Key, DatabaseName) must be provided.");
}
builder.Services.AddDbContext<ValuationDbContext>(opts =>
    opts.UseCosmos(
        cosmosSettings.Account,
        cosmosSettings.Key,
        cosmosSettings.DatabaseName));

// 3. Register repositories
builder.Services.AddScoped<IValuationRepository, ValuationRepository>();
builder.Services.AddScoped<IStakeholderRepository, StakeholderRepository>();

// 4. Configure StorageSettings and BlobServiceClient for uploads
builder.Services.Configure<StorageSettings>(
    builder.Configuration.GetSection("Storage"));

builder.Services.AddSingleton(sp =>
{
    var storageSettings = sp.GetRequiredService<IOptions<StorageSettings>>().Value;
    return new BlobServiceClient(storageSettings.ConnectionString);
});

// 5. Add controllers, API explorer & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 6. Ensure Cosmos DB container exists and seed lookup data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ValuationDbContext>();
    await db.Database.EnsureCreatedAsync();

    // Fetch to client before checking Any()
    var existingComponents = await db.ComponentTypes.ToListAsync();
    if (!existingComponents.Any())
    {
        db.ComponentTypes.AddRange(new[] {
            new ComponentType { Id = 1, Name = "ENGINE CONDITION", Description = "Status of engine internals" },
            new ComponentType { Id = 2, Name = "BRAKE SYSTEM", Description = "Condition of brakes" },
            // … additional items …
        });
        await db.SaveChangesAsync();
    }

    var existingSteps = await db.WorkflowStepTemplates.ToListAsync();
    if (!existingSteps.Any())
    {
        db.WorkflowStepTemplates.AddRange(new[] {
            new WorkflowStepTemplate { Id = 101, StepOrder = 1, StepName = "STEP - 1", Description = "Stake holder will raise the request in his portal" },
            new WorkflowStepTemplate { Id = 102, StepOrder = 2, StepName = "STEP - 2", Description = "PRONTO back end team will assign this case to AVO team" },
            new WorkflowStepTemplate { Id = 103, StepOrder = 3, StepName = "STEP - 3", Description = "PRONTO AVO team will complete the valuation in App" },
            new WorkflowStepTemplate { Id = 104, StepOrder = 4, StepName = "STEP - 4", Description = "PRONTO BACK END TEAM will carry out the data migration" },
            new WorkflowStepTemplate { Id = 105, StepOrder = 5, StepName = "STEP - 5", Description = "PRONTO QC TEAM will carry quality check on the final report" },
        });
        await db.SaveChangesAsync();
    }
}

// 7. Middleware pipeline
app.UseHttpsRedirection();
app.UseAuthorization();

// 8. Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pronto API V1");
    c.RoutePrefix = string.Empty;
});

app.MapControllers();
await app.RunAsync();
