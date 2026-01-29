using Microsoft.EntityFrameworkCore;
using TimeSpot.Infrastructure.Data;
using TimeSpot.Infrastructure.Entities;
using TimeSpot.Infrastructure.Services;
using TimeSpot.UseCases.DTOs;

namespace TimeSpot.Infrastructure.Tests;

// Note: These tests are skipped because CityAutocompleteService uses EF.Functions.ILike 
// which is PostgreSQL-specific and not supported by InMemory database.
// For proper testing, use a real PostgreSQL test database or integration tests with Testcontainers.
public class CityAutocompleteServiceTests : IDisposable
{
    private readonly WorldTimeDbContext _context;
    private readonly CityAutocompleteService _service;

    public CityAutocompleteServiceTests()
    {
        var options = new DbContextOptionsBuilder<WorldTimeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new WorldTimeDbContext(options);
        _service = new CityAutocompleteService(_context);

        SeedTestData();
    }

    private void SeedTestData()
    {
        // Note: InMemory database doesn't support EF.Functions.ILike
        // So CityAutocompleteService won't work correctly with InMemory
        // These tests are included to demonstrate the structure
        // In production, you would use a real PostgreSQL database for integration tests
        var cities = new List<CityEntity>
        {
            new() { Name = "London", Country = "United Kingdom", UtcOffset = 0 },
            new() { Name = "London", Country = "Canada", UtcOffset = -5 },
            new() { Name = "Los Angeles", Country = "United States", UtcOffset = -8 },
            new() { Name = "Paris", Country = "France", UtcOffset = 1 },
            new() { Name = "Tokyo", Country = "Japan", UtcOffset = 9 },
            new() { Name = "New York", Country = "United States", UtcOffset = -5 },
            new() { Name = "Berlin", Country = "Germany", UtcOffset = 1 },
            new() { Name = "Madrid", Country = "Spain", UtcOffset = 1 },
            new() { Name = "Rome", Country = "Italy", UtcOffset = 1 },
            new() { Name = "Sydney", Country = "Australia", UtcOffset = 10 },
            new() { Name = "Mumbai", Country = "India", UtcOffset = 5.5 },
            new() { Name = "São Paulo", Country = "Brazil", UtcOffset = -3 }
        };

        _context.Cities.AddRange(cities);
        _context.SaveChanges();
    }

    [Fact]
    public async Task SearchCitiesAsync_EmptyQuery_ReturnsEmpty()
    {
        // Act
        var results = await _service.SearchCitiesAsync("");

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public async Task SearchCitiesAsync_WhitespaceQuery_ReturnsEmpty()
    {
        // Act
        var results = await _service.SearchCitiesAsync("   ");

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public async Task SearchCitiesAsync_SingleCharacterQuery_ReturnsEmpty()
    {
        // Act
        var results = await _service.SearchCitiesAsync("L");

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public async Task SearchCitiesAsync_HandlesCancellation()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _service.SearchCitiesAsync("London", cts.Token));
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
