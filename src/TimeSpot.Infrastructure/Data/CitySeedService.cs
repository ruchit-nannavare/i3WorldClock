using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TimeSpot.Infrastructure.Entities;

namespace TimeSpot.Infrastructure.Data;

public class CitySeedService
{
    private readonly WorldTimeDbContext _context;

    public CitySeedService(WorldTimeDbContext context)
    {
        _context = context;
    }

    public async Task SeedAsync(string jsonFilePath)
    {
        // Check if already seeded
        if (await _context.Cities.AnyAsync())
        {
            return;
        }

        var jsonContent = await File.ReadAllTextAsync(jsonFilePath);
        var citiesData = JsonSerializer.Deserialize<CitiesJsonRoot>(jsonContent);

        if (citiesData?.Cities == null || citiesData.Cities.Count == 0)
        {
            throw new InvalidOperationException("No cities found in JSON file");
        }

        var entities = citiesData.Cities.Select(c => new CityEntity
        {
            Name = c.Name,
            Country = c.Country,
            UtcOffset = c.UtcOffset
        }).ToList();

        await _context.Cities.AddRangeAsync(entities);
        await _context.SaveChangesAsync();
    }
}

// JSON deserialization models
public class CitiesJsonRoot
{
    [JsonPropertyName("cities")]
    public List<CityJson> Cities { get; set; } = new();
}

public class CityJson
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;

    [JsonPropertyName("utcOffset")]
    public double UtcOffset { get; set; }
}
