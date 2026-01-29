using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using TimeSpot.Core.Enums;
using TimeSpot.Server.Controllers;
using TimeSpot.UseCases.DTOs;
using TimeSpot.UseCases.Interfaces;

namespace TimeSpot.Server.Tests.Controllers;

public class WeatherControllerTests
{
    private readonly IWeatherService _mockWeatherService;
    private readonly WeatherController _controller;

    public WeatherControllerTests()
    {
        _mockWeatherService = Substitute.For<IWeatherService>();
        _controller = new WeatherController(_mockWeatherService);
    }

    [Fact]
    public async Task GetWeather_WithValidCoordinates_ReturnsOkWithWeather()
    {
        // Arrange
        var expectedWeather = new WeatherDto(
            Temperature: 22.5,
            Condition: WeatherCondition.Clear,
            DayNight: DayNightStatus.Day,
            Sunrise: "06:30",
            Sunset: "18:45"
        );
        _mockWeatherService.GetWeatherAsync(51.5074, -0.1278, Arg.Any<CancellationToken>())
            .Returns(expectedWeather);

        // Act
        var result = await _controller.GetWeather(51.5074, -0.1278, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var weather = Assert.IsType<WeatherDto>(okResult.Value);
        Assert.Equal(22.5, weather.Temperature);
        Assert.Equal(WeatherCondition.Clear, weather.Condition);
        Assert.Equal(DayNightStatus.Day, weather.DayNight);
        await _mockWeatherService.Received(1).GetWeatherAsync(51.5074, -0.1278, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetWeather_WithNegativeLatitude_ReturnsWeather()
    {
        // Arrange
        var expectedWeather = new WeatherDto(15.0, WeatherCondition.Cloudy, DayNightStatus.Night, "07:00", "19:00");
        _mockWeatherService.GetWeatherAsync(-33.8688, 151.2093, Arg.Any<CancellationToken>())
            .Returns(expectedWeather);

        // Act
        var result = await _controller.GetWeather(-33.8688, 151.2093, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var weather = Assert.IsType<WeatherDto>(okResult.Value);
        Assert.Equal(15.0, weather.Temperature);
    }

    [Fact]
    public async Task GetWeather_WithNegativeLongitude_ReturnsWeather()
    {
        // Arrange
        var expectedWeather = new WeatherDto(18.0, WeatherCondition.Rain, DayNightStatus.Day, "06:45", "18:30");
        _mockWeatherService.GetWeatherAsync(40.7128, -74.0060, Arg.Any<CancellationToken>())
            .Returns(expectedWeather);

        // Act
        var result = await _controller.GetWeather(40.7128, -74.0060, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.IsType<WeatherDto>(okResult.Value);
    }

    [Fact]
    public async Task GetWeather_PassesCancellationTokenToService()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var expectedWeather = new WeatherDto(20.0, WeatherCondition.Clear, DayNightStatus.Day, "06:30", "18:45");
        _mockWeatherService.GetWeatherAsync(Arg.Any<double>(), Arg.Any<double>(), cancellationToken)
            .Returns(expectedWeather);

        // Act
        await _controller.GetWeather(51.5, -0.1, cancellationToken);

        // Assert
        await _mockWeatherService.Received(1).GetWeatherAsync(51.5, -0.1, cancellationToken);
    }

    [Fact]
    public async Task GetWeather_ServiceThrowsException_PropagatesException()
    {
        // Arrange
        _mockWeatherService.GetWeatherAsync(Arg.Any<double>(), Arg.Any<double>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<WeatherDto>(new HttpRequestException("API unavailable")));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(() => _controller.GetWeather(51.5, -0.1, CancellationToken.None));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(90, 180)]
    [InlineData(-90, -180)]
    [InlineData(45.5, 120.3)]
    public async Task GetWeather_WithVariousCoordinates_CallsServiceWithCorrectParameters(double lat, double lon)
    {
        // Arrange
        var expectedWeather = new WeatherDto(20.0, WeatherCondition.Clear, DayNightStatus.Day, "06:30", "18:45");
        _mockWeatherService.GetWeatherAsync(lat, lon, Arg.Any<CancellationToken>())
            .Returns(expectedWeather);

        // Act
        await _controller.GetWeather(lat, lon, CancellationToken.None);

        // Assert
        await _mockWeatherService.Received(1).GetWeatherAsync(lat, lon, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetWeather_WithAllWeatherConditions_ReturnsCorrectCondition()
    {
        // Arrange
        foreach (WeatherCondition condition in Enum.GetValues(typeof(WeatherCondition)))
        {
            var expectedWeather = new WeatherDto(20.0, condition, DayNightStatus.Day, "06:30", "18:45");
            _mockWeatherService.GetWeatherAsync(Arg.Any<double>(), Arg.Any<double>(), Arg.Any<CancellationToken>())
                .Returns(expectedWeather);

            // Act
            var result = await _controller.GetWeather(51.5, -0.1, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var weather = Assert.IsType<WeatherDto>(okResult.Value);
            Assert.Equal(condition, weather.Condition);
        }
    }
}
