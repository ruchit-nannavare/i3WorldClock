using Microsoft.AspNetCore.Mvc;
using TimeSpot.Server.Controllers;
using TimeSpot.UseCases.DTOs;
using TimeSpot.UseCases.Services;

namespace TimeSpot.Server.Tests.Controllers;

public class TimeControllerTests
{
    [Fact]
    public void GetUtcTime_ReturnsOkWithUtcTime()
    {
        // Arrange
        var timeService = new TimeService();
        var controller = new TimeController(timeService);
        var beforeCall = DateTime.UtcNow;

        // Act
        var result = controller.GetUtcTime();

        // Assert
        var afterCall = DateTime.UtcNow;
        var okResult = Assert.IsType<OkObjectResult>(result);
        var utcTime = Assert.IsType<UtcTimeDto>(okResult.Value);
        Assert.True(utcTime.UtcTime >= beforeCall && utcTime.UtcTime <= afterCall);
        Assert.True(utcTime.UnixTimestamp > 0);
    }

    [Fact]
    public void GetUtcTime_TimestampIsValid()
    {
        // Arrange
        var timeService = new TimeService();
        var controller = new TimeController(timeService);

        // Act
        var result = controller.GetUtcTime();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var utcTime = Assert.IsType<UtcTimeDto>(okResult.Value);
        var expectedTimestamp = new DateTimeOffset(utcTime.UtcTime).ToUnixTimeSeconds();
        Assert.Equal(expectedTimestamp, utcTime.UnixTimestamp);
    }

    [Fact]
    public void GetUtcTime_ReturnsValidResponseStructure()
    {
        // Arrange
        var timeService = new TimeService();
        var controller = new TimeController(timeService);

        // Act
        var result = controller.GetUtcTime();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value);
        var utcTime = Assert.IsType<UtcTimeDto>(okResult.Value);
        Assert.NotEqual(default, utcTime.UtcTime);
        Assert.True(utcTime.UnixTimestamp > 0);
    }

    [Fact]
    public void GetUtcTime_CalledMultipleTimes_TimestampsIncrease()
    {
        // Arrange
        var timeService = new TimeService();
        var controller = new TimeController(timeService);

        // Act
        var result1 = controller.GetUtcTime();
        System.Threading.Thread.Sleep(100); // Small delay to ensure time difference
        var result2 = controller.GetUtcTime();

        // Assert
        var okResult1 = Assert.IsType<OkObjectResult>(result1);
        var okResult2 = Assert.IsType<OkObjectResult>(result2);
        var utcTime1 = Assert.IsType<UtcTimeDto>(okResult1.Value);
        var utcTime2 = Assert.IsType<UtcTimeDto>(okResult2.Value);
        Assert.True(utcTime2.UnixTimestamp >= utcTime1.UnixTimestamp);
    }

    [Fact]
    public void GetUtcTime_ReturnsUtcTimeDto()
    {
        // Arrange
        var timeService = new TimeService();
        var controller = new TimeController(timeService);

        // Act
        var result = controller.GetUtcTime();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var utcTime = Assert.IsType<UtcTimeDto>(okResult.Value);
        Assert.Equal(DateTimeKind.Utc, utcTime.UtcTime.Kind);
    }
}
