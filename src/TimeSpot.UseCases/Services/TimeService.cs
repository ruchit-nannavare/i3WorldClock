using TimeSpot.UseCases.DTOs;

namespace TimeSpot.UseCases.Services;

public class TimeService
{
    public UtcTimeDto GetUtcTime()
    {
        var utcNow = DateTime.UtcNow;
        var unixTimestamp = new DateTimeOffset(utcNow).ToUnixTimeSeconds();
        return new UtcTimeDto(utcNow, unixTimestamp);
    }
}
