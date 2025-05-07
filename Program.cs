using Pronto.ValuationApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure Cosmos DB
var cosmos = builder.Configuration.GetSection("Cosmos");
builder.Services.AddDbContext<ProntoDbContext>(opts =>
  opts.UseCosmos(
    cosmos["Account"] ?? throw new ArgumentNullException("Cosmos:Account"),
    cosmos["Key"] ?? throw new ArgumentNullException("Cosmos:Key"),
    databaseName: cosmos["Database"] ?? throw new ArgumentNullException("Cosmos:Database")));

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
        // optional: c.RoutePrefix = string.Empty; // serve at root
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
