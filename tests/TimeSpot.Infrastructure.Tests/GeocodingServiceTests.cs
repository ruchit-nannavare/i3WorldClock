using System.Net;
using TimeSpot.Infrastructure.ExternalServices;

namespace TimeSpot.Infrastructure.Tests;

public class GeocodingServiceTests
{
    [Fact]
    public async Task SearchCitiesAsync_ReturnsFormattedCities()
    {
        // Arrange
        var responseJson = """
        {
            "results": [
                {
                    "name": "London",
                    "country": "United Kingdom",
                    "country_code": "GB",
                    "latitude": 51.5074,
                    "longitude": -0.1278,
                    "timezone": "Europe/London"
                }
            ]
        }
        """;

        var httpClient = CreateMockHttpClient(responseJson);
        var service = new GeocodingService(httpClient);

        // Act
        var results = await service.SearchCitiesAsync("London");
        var city = results.First();

        // Assert
        Assert.Equal("london-gb", city.Id);
        Assert.Equal("London", city.Name);
        Assert.Equal("United Kingdom", city.Country);
        Assert.Equal("GB", city.CountryCode);
        Assert.Equal(51.5074, city.Latitude);
        Assert.Equal(-0.1278, city.Longitude);
    }

    [Fact]
    public async Task SearchCitiesAsync_EmptyQuery_ReturnsEmpty()
    {
        // Arrange
        var httpClient = CreateMockHttpClient("{}");
        var service = new GeocodingService(httpClient);

        // Act
        var results = await service.SearchCitiesAsync("");

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public async Task SearchCitiesAsync_ShortQuery_ReturnsEmpty()
    {
        // Arrange
        var httpClient = CreateMockHttpClient("{}");
        var service = new GeocodingService(httpClient);

        // Act
        var results = await service.SearchCitiesAsync("L");

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public async Task SearchCitiesAsync_NoResults_ReturnsEmpty()
    {
        // Arrange
        var responseJson = """
        {
            "results": null
        }
        """;

        var httpClient = CreateMockHttpClient(responseJson);
        var service = new GeocodingService(httpClient);

        // Act
        var results = await service.SearchCitiesAsync("xyznonexistent");

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public async Task SearchCitiesAsync_GeneratesCityIdCorrectly()
    {
        // Arrange
        var responseJson = """
        {
            "results": [
                {
                    "name": "New York",
                    "country": "United States",
                    "country_code": "US",
                    "latitude": 40.7128,
                    "longitude": -74.0060,
                    "timezone": "America/New_York"
                }
            ]
        }
        """;

        var httpClient = CreateMockHttpClient(responseJson);
        var service = new GeocodingService(httpClient);

        // Act
        var results = await service.SearchCitiesAsync("New York");
        var city = results.First();

        // Assert
        Assert.Equal("new-york-us", city.Id);
    }

    private static HttpClient CreateMockHttpClient(string responseContent)
    {
        var handler = new MockHttpMessageHandler(responseContent);
        return new HttpClient(handler)
        {
            BaseAddress = new Uri("https://geocoding-api.open-meteo.com")
        };
    }
}
