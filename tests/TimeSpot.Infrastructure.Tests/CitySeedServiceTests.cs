using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TimeSpot.Infrastructure.Data;
using TimeSpot.Infrastructure.Entities;

namespace TimeSpot.Infrastructure.Tests;

public class CitySeedServiceTests : IDisposable
{
    private readonly WorldTimeDbContext _context;
    private readonly CitySeedService _service;
    private readonly string _testJsonPath;

    public CitySeedServiceTests()
    {
        var options = new DbContextOptionsBuilder<WorldTimeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new WorldTimeDbContext(options);
        _service = new CitySeedService(_context);
        _testJsonPath = Path.Combine(Path.GetTempPath(), $"test_cities_{Guid.NewGuid()}.json");
    }

    [Fact]
    public async Task SeedAsync_WithValidJson_SeedsCities()
    {
        // Arrange
        var citiesData = new
        {
            cities = new[]
            {
                new { name = "London", country = "United Kingdom", utcOffset = 0.0 },
                new { name = "Paris", country = "France", utcOffset = 1.0 },
                new { name = "Tokyo", country = "Japan", utcOffset = 9.0 }
            }
        };
        await File.WriteAllTextAsync(_testJsonPath, JsonSerializer.Serialize(citiesData));

        // Act
        await _service.SeedAsync(_testJsonPath);

        // Assert
        var cities = await _context.Cities.ToListAsync();
        Assert.Equal(3, cities.Count);
        Assert.Contains(cities, c => c.Name == "London" && c.Country == "United Kingdom");
        Assert.Contains(cities, c => c.Name == "Paris" && c.Country == "France");
        Assert.Contains(cities, c => c.Name == "Tokyo" && c.Country == "Japan");
    }

    [Fact]
    public async Task SeedAsync_DatabaseAlreadyHasCities_DoesNotSeed()
    {
        // Arrange
        _context.Cities.Add(new CityEntity { Name = "ExistingCity", Country = "ExistingCountry", UtcOffset = 0 });
        await _context.SaveChangesAsync();

        var citiesData = new
        {
            cities = new[]
            {
                new { name = "London", country = "United Kingdom", utcOffset = 0.0 }
            }
        };
        await File.WriteAllTextAsync(_testJsonPath, JsonSerializer.Serialize(citiesData));

        // Act
        await _service.SeedAsync(_testJsonPath);

        // Assert
        var cities = await _context.Cities.ToListAsync();
        Assert.Single(cities);
        Assert.Equal("ExistingCity", cities[0].Name);
    }

    [Fact]
    public async Task SeedAsync_EmptyCitiesArray_ThrowsException()
    {
        // Arrange
        var citiesData = new { cities = Array.Empty<object>() };
        await File.WriteAllTextAsync(_testJsonPath, JsonSerializer.Serialize(citiesData));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.SeedAsync(_testJsonPath));
    }

    [Fact]
    public async Task SeedAsync_NullCitiesArray_ThrowsException()
    {
        // Arrange
        var citiesData = new { cities = (object?)null };
        await File.WriteAllTextAsync(_testJsonPath, JsonSerializer.Serialize(citiesData));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.SeedAsync(_testJsonPath));
    }

    [Fact]
    public async Task SeedAsync_InvalidJson_ThrowsJsonException()
    {
        // Arrange
        await File.WriteAllTextAsync(_testJsonPath, "{ invalid json }");

        // Act & Assert
        await Assert.ThrowsAsync<JsonException>(() => _service.SeedAsync(_testJsonPath));
    }

    [Fact]
    public async Task SeedAsync_FileDoesNotExist_ThrowsFileNotFoundException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<FileNotFoundException>(() => _service.SeedAsync("nonexistent.json"));
    }

    [Fact]
    public async Task SeedAsync_CorrectlyMapsUtcOffsets()
    {
        // Arrange
        var citiesData = new
        {
            cities = new[]
            {
                new { name = "City1", country = "Country1", utcOffset = 5.5 },
                new { name = "City2", country = "Country2", utcOffset = -3.0 },
                new { name = "City3", country = "Country3", utcOffset = 0.0 }
            }
        };
        await File.WriteAllTextAsync(_testJsonPath, JsonSerializer.Serialize(citiesData));

        // Act
        await _service.SeedAsync(_testJsonPath);

        // Assert
        var cities = await _context.Cities.ToListAsync();
        Assert.Equal(5.5, cities.First(c => c.Name == "City1").UtcOffset);
        Assert.Equal(-3.0, cities.First(c => c.Name == "City2").UtcOffset);
        Assert.Equal(0.0, cities.First(c => c.Name == "City3").UtcOffset);
    }

    [Fact]
    public async Task SeedAsync_HandlesSpecialCharactersInNames()
    {
        // Arrange
        var citiesData = new
        {
            cities = new[]
            {
                new { name = "São Paulo", country = "Brazil", utcOffset = -3.0 },
                new { name = "Zürich", country = "Switzerland", utcOffset = 1.0 }
            }
        };
        await File.WriteAllTextAsync(_testJsonPath, JsonSerializer.Serialize(citiesData));

        // Act
        await _service.SeedAsync(_testJsonPath);

        // Assert
        var cities = await _context.Cities.ToListAsync();
        Assert.Equal(2, cities.Count);
        Assert.Contains(cities, c => c.Name == "São Paulo");
        Assert.Contains(cities, c => c.Name == "Zürich");
    }

    [Fact]
    public async Task SeedAsync_LargeDataset_SeedsAllCities()
    {
        // Arrange
        var largeCityList = Enumerable.Range(1, 500)
            .Select(i => new { name = $"City{i}", country = $"Country{i}", utcOffset = (double)(i % 24 - 12) })
            .ToArray();

        var citiesData = new { cities = largeCityList };
        await File.WriteAllTextAsync(_testJsonPath, JsonSerializer.Serialize(citiesData));

        // Act
        await _service.SeedAsync(_testJsonPath);

        // Assert
        var cities = await _context.Cities.ToListAsync();
        Assert.Equal(500, cities.Count);
    }

    [Fact]
    public async Task SeedAsync_DuplicateCityNames_SeedsBoth()
    {
        // Arrange
        var citiesData = new
        {
            cities = new[]
            {
                new { name = "London", country = "United Kingdom", utcOffset = 0.0 },
                new { name = "London", country = "Canada", utcOffset = -5.0 }
            }
        };
        await File.WriteAllTextAsync(_testJsonPath, JsonSerializer.Serialize(citiesData));

        // Act
        await _service.SeedAsync(_testJsonPath);

        // Assert
        var cities = await _context.Cities.ToListAsync();
        Assert.Equal(2, cities.Count);
        Assert.All(cities, c => Assert.Equal("London", c.Name));
    }

    [Fact]
    public async Task SeedAsync_CalledTwiceOnEmptyDatabase_OnlySeedsOnce()
    {
        // Arrange
        var citiesData = new
        {
            cities = new[]
            {
                new { name = "TestCity", country = "TestCountry", utcOffset = 0.0 }
            }
        };
        await File.WriteAllTextAsync(_testJsonPath, JsonSerializer.Serialize(citiesData));

        // Act
        await _service.SeedAsync(_testJsonPath);
        var countAfterFirst = await _context.Cities.CountAsync();
        
        await _service.SeedAsync(_testJsonPath);
        var countAfterSecond = await _context.Cities.CountAsync();

        // Assert
        Assert.Equal(1, countAfterFirst);
        Assert.Equal(1, countAfterSecond);
    }

    public void Dispose()
    {
        if (File.Exists(_testJsonPath))
        {
            File.Delete(_testJsonPath);
        }
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
