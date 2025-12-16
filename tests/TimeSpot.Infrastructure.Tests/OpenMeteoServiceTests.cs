using System.Net;
using System.Text.Json;
using NSubstitute;
using TimeSpot.Core.Enums;
using TimeSpot.Infrastructure.ExternalServices;

namespace TimeSpot.Infrastructure.Tests;

public class OpenMeteoServiceTests
{
    [Fact]
    public async Task GetWeatherAsync_ParsesResponseCorrectly()
    {
        // Arrange
        var responseJson = """
        {
            "current": {
                "temperature_2m": 22.5,
                "weather_code": 0,
                "is_day": 1
            },
            "daily": {
                "sunrise": ["2024-01-15T07:12"],
                "sunset": ["2024-01-15T17:45"]
            }
        }
        """;

        var httpClient = CreateMockHttpClient(responseJson);
        var service = new OpenMeteoService(httpClient);

        // Act
        var result = await service.GetWeatherAsync(51.5074, -0.1278);

        // Assert
        Assert.Equal(22.5, result.Temperature);
        Assert.Equal(WeatherCondition.Clear, result.Condition);
        Assert.Equal(DayNightStatus.Day, result.DayNight);
        Assert.Equal("07:12", result.Sunrise);
        Assert.Equal("17:45", result.Sunset);
    }

    [Fact]
    public async Task GetWeatherAsync_NightTime_ReturnsDayNightStatusNight()
    {
        // Arrange
        var responseJson = """
        {
            "current": {
                "temperature_2m": 15.0,
                "weather_code": 0,
                "is_day": 0
            },
            "daily": {
                "sunrise": ["2024-01-15T07:12"],
                "sunset": ["2024-01-15T17:45"]
            }
        }
        """;

        var httpClient = CreateMockHttpClient(responseJson);
        var service = new OpenMeteoService(httpClient);

        // Act
        var result = await service.GetWeatherAsync(51.5074, -0.1278);

        // Assert
        Assert.Equal(DayNightStatus.Night, result.DayNight);
    }

    [Theory]
    [InlineData(0, WeatherCondition.Clear)]
    [InlineData(1, WeatherCondition.PartlyCloudy)]
    [InlineData(2, WeatherCondition.PartlyCloudy)]
    [InlineData(3, WeatherCondition.Cloudy)]
    [InlineData(45, WeatherCondition.Fog)]
    [InlineData(48, WeatherCondition.Fog)]
    [InlineData(61, WeatherCondition.Rain)]
    [InlineData(63, WeatherCondition.Rain)]
    [InlineData(65, WeatherCondition.Rain)]
    [InlineData(71, WeatherCondition.Snow)]
    [InlineData(73, WeatherCondition.Snow)]
    [InlineData(95, WeatherCondition.Thunderstorm)]
    [InlineData(99, WeatherCondition.Thunderstorm)]
    public async Task GetWeatherAsync_MapsWeatherCodeCorrectly(int weatherCode, WeatherCondition expectedCondition)
    {
        // Arrange
        var responseJson = $$"""
        {
            "current": {
                "temperature_2m": 20.0,
                "weather_code": {{weatherCode}},
                "is_day": 1
            },
            "daily": {
                "sunrise": ["2024-01-15T07:12"],
                "sunset": ["2024-01-15T17:45"]
            }
        }
        """;

        var httpClient = CreateMockHttpClient(responseJson);
        var service = new OpenMeteoService(httpClient);

        // Act
        var result = await service.GetWeatherAsync(51.5074, -0.1278);

        // Assert
        Assert.Equal(expectedCondition, result.Condition);
    }

    private static HttpClient CreateMockHttpClient(string responseContent)
    {
        var handler = new MockHttpMessageHandler(responseContent);
        return new HttpClient(handler)
        {
            BaseAddress = new Uri("https://api.open-meteo.com")
        };
    }
}

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly string _responseContent;

    public MockHttpMessageHandler(string responseContent)
    {
        _responseContent = responseContent;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(_responseContent, System.Text.Encoding.UTF8, "application/json")
        };
        return Task.FromResult(response);
    }
}
