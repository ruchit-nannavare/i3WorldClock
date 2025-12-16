using TimeSpot.UseCases.Services;

namespace TimeSpot.UseCases.Tests;

public class TimeServiceTests
{
    [Fact]
    public void GetUtcTime_ReturnsCurrentUtcTime()
    {
        // Arrange
        var service = new TimeService();
        var beforeCall = DateTime.UtcNow;

        // Act
        var result = service.GetUtcTime();

        // Assert
        var afterCall = DateTime.UtcNow;
        Assert.True(result.UtcTime >= beforeCall && result.UtcTime <= afterCall);
    }

    [Fact]
    public void GetUtcTime_ReturnsValidUnixTimestamp()
    {
        // Arrange
        var service = new TimeService();

        // Act
        var result = service.GetUtcTime();

        // Assert
        var expectedTimestamp = new DateTimeOffset(result.UtcTime).ToUnixTimeSeconds();
        Assert.Equal(expectedTimestamp, result.UnixTimestamp);
    }

    [Fact]
    public void GetUtcTime_TimestampIsPositive()
    {
        // Arrange
        var service = new TimeService();

        // Act
        var result = service.GetUtcTime();

        // Assert
        Assert.True(result.UnixTimestamp > 0);
    }
}
