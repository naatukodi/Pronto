using Pronto.ValuationApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Read the full connection string
var connectionString = builder.Configuration.GetConnectionString("CosmosDb")
    ?? throw new InvalidOperationException("Connection string for 'CosmosDb' is not configured.");

// Read just the database name
var databaseName = builder.Configuration["Cosmos:Database"];

// Configure DbContext with CosmosDb
builder.Services.AddDbContext<ProntoDbContext>(opts =>
{
    opts.UseCosmos(connectionString, databaseName ?? throw new InvalidOperationException("Database name for 'Cosmos' is not configured."));
});

// Add controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// only in dev
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pronto API V1");
        c.RoutePrefix = string.Empty; // serve at root
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
