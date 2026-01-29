using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using TimeSpot.Server.Controllers;
using TimeSpot.UseCases.DTOs;
using TimeSpot.UseCases.Interfaces;

namespace TimeSpot.Server.Tests.Controllers;

public class CitiesControllerTests
{
    private readonly ICitySearchService _mockCitySearchService;
    private readonly CitiesController _controller;

    public CitiesControllerTests()
    {
        _mockCitySearchService = Substitute.For<ICitySearchService>();
        _controller = new CitiesController(_mockCitySearchService);
    }

    [Fact]
    public async Task Search_WithValidQuery_ReturnsOkWithCities()
    {
        // Arrange
        var expectedCities = new List<CityDto>
        {
            new CityDto("london-gb", "London", "United Kingdom", "GB", 51.5074, -0.1278, 0, "UTC+0"),
            new CityDto("london-ca", "London", "Canada", "CA", 42.9849, -81.2453, -18000, "UTC-5")
        };
        _mockCitySearchService.SearchCitiesAsync("London", Arg.Any<CancellationToken>())
            .Returns(expectedCities);

        // Act
        var result = await _controller.Search("London", CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var cities = Assert.IsAssignableFrom<IEnumerable<CityDto>>(okResult.Value);
        Assert.Equal(2, cities.Count());
        await _mockCitySearchService.Received(1).SearchCitiesAsync("London", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Search_WithEmptyQuery_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.Search("", CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Query parameter is required", badRequestResult.Value);
        await _mockCitySearchService.DidNotReceive().SearchCitiesAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Search_WithNullQuery_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.Search(null!, CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Query parameter is required", badRequestResult.Value);
        await _mockCitySearchService.DidNotReceive().SearchCitiesAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Search_WithWhitespaceQuery_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.Search("   ", CancellationToken.None);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Query parameter is required", badRequestResult.Value);
        await _mockCitySearchService.DidNotReceive().SearchCitiesAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Search_WithNoResults_ReturnsOkWithEmptyList()
    {
        // Arrange
        _mockCitySearchService.SearchCitiesAsync("xyz123", Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<CityDto>());

        // Act
        var result = await _controller.Search("xyz123", CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var cities = Assert.IsAssignableFrom<IEnumerable<CityDto>>(okResult.Value);
        Assert.Empty(cities);
    }

    [Fact]
    public async Task Search_PassesCancellationTokenToService()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        _mockCitySearchService.SearchCitiesAsync(Arg.Any<string>(), cancellationToken)
            .Returns(Enumerable.Empty<CityDto>());

        // Act
        await _controller.Search("test", cancellationToken);

        // Assert
        await _mockCitySearchService.Received(1).SearchCitiesAsync("test", cancellationToken);
    }

    [Fact]
    public async Task Search_ServiceThrowsException_PropagatesException()
    {
        // Arrange
        _mockCitySearchService.SearchCitiesAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromException<IEnumerable<CityDto>>(new Exception("Database error")));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _controller.Search("London", CancellationToken.None));
    }
}
