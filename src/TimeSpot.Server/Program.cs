using Microsoft.EntityFrameworkCore;
using TimeSpot.Infrastructure.Data;
using TimeSpot.Infrastructure.ExternalServices;
using TimeSpot.Infrastructure.Services;
using TimeSpot.UseCases.Interfaces;
using TimeSpot.UseCases.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Configure PostgreSQL with Entity Framework
var connectionString = builder.Configuration.GetConnectionString("WorldTimeDb");
builder.Services.AddDbContext<WorldTimeDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register HttpClient for external services
builder.Services.AddHttpClient<IWeatherService, OpenMeteoService>();

// Register city search service (uses PostgreSQL database)
builder.Services.AddScoped<ICitySearchService, CityAutocompleteService>();

// Register seeder service
builder.Services.AddScoped<CitySeedService>();

// Register application services
builder.Services.AddSingleton<TimeService>();

// Configure CORS for React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:5174", "http://localhost:3000")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Apply migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WorldTimeDbContext>();
    await dbContext.Database.MigrateAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<CitySeedService>();
    var citiesJsonPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "TimeSpot.Infrastructure", "Data", "cities.json");

    // Try to find the cities.json file
    if (!File.Exists(citiesJsonPath))
    {
        // Try alternative path for development
        citiesJsonPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "TimeSpot.Infrastructure", "Data", "cities.json");
    }

    if (File.Exists(citiesJsonPath))
    {
        await seeder.SeedAsync(citiesJsonPath);
    }
}

// Configure the HTTP request pipeline
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
