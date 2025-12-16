namespace TimeSpot.UseCases.DTOs;

public record UtcTimeDto(
    DateTime UtcTime,
    long UnixTimestamp
);
